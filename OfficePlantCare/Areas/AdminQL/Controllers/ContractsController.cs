using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OfficePlantCare.Models;
using X.PagedList.Extensions;

namespace OfficePlantCare.Areas.AdminQL.Controllers
{
    public class ContractsController : BaseController
    {
        private readonly OfficePlantCareContext _context;
        private readonly CareScheduleController _careSchedulesController;
        private readonly List<int> validContractServiceIds = new List<int> { 1, 2, 3, 6, 12, 14 };

        public ContractsController(OfficePlantCareContext context, CareScheduleController careSchedulesController)
        {
            _context = context;
            _careSchedulesController = careSchedulesController;
        }

        // GET: AdminQL/Contracts
        public async Task<IActionResult> Index(string name, int page = 1, int? confirmPayment = null)
        {
            // Số ghi trên 1 trang
            int limit = 5;

            // Tạo query cơ bản
            IQueryable<Contract> query = _context.Contracts
                                        .Include(c => c.Customer)
                                        .Include(c => c.Price)
                                        .Include(c => c.PaymentMethod)
                                        .Include(c => c.Service)
                                        .OrderBy(c => c.Customer.CustomerName);

            // Nếu có tham số name trên URL, thêm điều kiện lọc
            if (!string.IsNullOrEmpty(name))
            {
                query = query.Where(c => c.Customer.CustomerName.Contains(name));
            }

            // Xử lý xác nhận thanh toán
            if (confirmPayment.HasValue)
            {
                var contracts = await _context.Contracts
                    .Where(s => s.CustomerId == confirmPayment.Value)
                    .ToListAsync();

                if (contracts.Any())
                {
                    foreach (var ccontract in contracts)
                    {
                        if (ccontract.PaymentStatus == "Chưa thanh toán")
                        {
                            ccontract.PaymentStatus = "Đã thanh toán";
                            ccontract.StartDate = DateTime.Now;
                            _context.Update(ccontract);
                        }
                    }
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Xác nhận thanh toán thành công cho tất cả đơn hàng của khách hàng!";
                }
                else
                {
                    TempData["ErrorMessage"] = "Không tìm thấy yêu cầu nào để cập nhật cho khách hàng này.";
                }
            }

            // Chuyển query sang danh sách
            var contract = await query.ToListAsync(); // Dùng ToListAsync() của EF Core

            // Sử dụng ToPagedList để phân trang (không bất đồng bộ)
            var pagedContract = contract.ToPagedList(page, limit);

            // Gửi từ khóa tìm kiếm cho View qua ViewBag
            ViewBag.keyword = name;

            return View(pagedContract);
        }

        // GET: AdminQL/Contracts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var contract = await _context.Contracts
                .Include(c => c.Customer)
                .Include(c => c.PaymentMethod)
                .Include(c => c.Price)
                .Include(c => c.Service)
                .FirstOrDefaultAsync(m => m.ContractId == id);
            if (contract == null)
            {
                return NotFound();
            }

            return PartialView("_Details", contract);
        }

        // GET: AdminQL/Contracts/Create
        public IActionResult Create()
        {
            var contractServices = _context.Services
                .Where(s => validContractServiceIds.Contains(s.ServiceId))
                .ToList();

            // Xử lý ServiceId nullable an toàn và chuyển đổi về List<object>
            ViewBag.ServicePrices = _context.ServicePrices
                .Where(sp => sp.ServiceId.HasValue && validContractServiceIds.Contains(sp.ServiceId.Value))
                .Select(sp => new
                {
                    sp.PriceId,
                    sp.ServiceId,
                    sp.ServiceType,
                    sp.TreeSize,
                    sp.OfficeSize,
                    sp.Price,
                    sp.DurationInMonths
                }).Cast<object>().ToList() ?? new List<object>();

            ViewData["CustomerId"] = new SelectList(_context.Customers.Where(c => c.RoleId == 4), "CustomerId", "CustomerName");
            ViewData["ServiceId"] = new SelectList(contractServices, "ServiceId", "ServiceName");
            ViewData["PaymentMethodId"] = new SelectList(_context.PaymentMethods, "PaymentMethodId", "MethodName");

            return View(new Contract());
        }

        // POST: AdminQL/Contracts/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind("ContractId,ContractCode,CustomerId,ServiceId,PriceId,DurationUnit,Duration,TotalAmount,StartDate,EndDate,PaymentMethodId,PaymentStatus,Status")] Contract contract,
            string CareDay1, string CareDay2) // Thêm tham số để nhận ngày chăm sóc từ form
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var servicePrice = _context.ServicePrices
                        .FirstOrDefault(sp => sp.PriceId == contract.PriceId);

                    if (servicePrice == null)
                    {
                        ModelState.AddModelError("", "Không tìm thấy thông tin giá.");
                    }
                    else
                    {
                        contract.ServiceId = (int)servicePrice.ServiceId;
                        contract.TotalAmount = servicePrice.Price ?? 0; // Xử lý null
                        contract.Duration = servicePrice.DurationInMonths ?? 0; // Xử lý null
                        contract.DurationUnit = "Tháng";

                        if (contract.StartDate.HasValue)
                        {
                            contract.EndDate = contract.StartDate.Value.AddMonths(servicePrice.DurationInMonths ?? 0);
                        }
                        else
                        {
                            ModelState.AddModelError("", "Vui lòng nhập ngày bắt đầu.");
                        }

                        contract.Status = "Chờ xử lý";
                        contract.PaymentStatus = "Chưa thanh toán";

                        _context.Add(contract);
                        await _context.SaveChangesAsync();

                        // Tự động tạo CareSchedule với tần suất 2 lần 1 tuần
                        DayOfWeek? day1 = null;
                        DayOfWeek? day2 = null;
                        if (!string.IsNullOrEmpty(CareDay1))
                        {
                            day1 = (DayOfWeek)Enum.Parse(typeof(DayOfWeek), CareDay1);
                        }
                        if (!string.IsNullOrEmpty(CareDay2))
                        {
                            day2 = (DayOfWeek)Enum.Parse(typeof(DayOfWeek), CareDay2);
                        }
                        await GenerateCareSchedules(contract, day1, day2);

                        // Thêm thông báo thành công vào TempData
                        TempData["SuccessMessage"] = "Thêm hợp đồng thành công! Lịch chăm sóc đã được tạo.";
                        return RedirectToAction(nameof(Index));
                    }
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"Lỗi: {ex.Message}");
                }
            }

            var contractServices = _context.Services
                .Where(s => validContractServiceIds.Contains(s.ServiceId))
                .ToList();

            ViewBag.ServicePrices = _context.ServicePrices
                .Where(sp => sp.ServiceId.HasValue && validContractServiceIds.Contains(sp.ServiceId.Value))
                .Select(sp => new
                {
                    sp.PriceId,
                    sp.ServiceId,
                    sp.ServiceType,
                    sp.TreeSize,
                    sp.OfficeSize,
                    sp.Price,
                    sp.DurationInMonths
                }).Cast<object>().ToList() ?? new List<object>();

            ViewData["CustomerId"] = new SelectList(_context.Customers.Where(c => c.RoleId == 4), "CustomerId", "CustomerName", contract.CustomerId);
            ViewData["ServiceId"] = new SelectList(contractServices, "ServiceId", "ServiceName", contract.ServiceId);
            ViewData["PaymentMethodId"] = new SelectList(_context.PaymentMethods, "PaymentMethodId", "MethodName", contract.PaymentMethodId);

            return View(contract);
        }

        // GET: AdminQL/Contracts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var contract = await _context.Contracts.FindAsync(id);
            if (contract == null)
            {
                return NotFound();
            }

            // Lọc dịch vụ hợp đồng theo danh sách validContractServiceIds
            var contractServices = _context.Services
                .Where(s => validContractServiceIds.Contains(s.ServiceId))
                .ToList();
            ViewData["CustomerId"] = new SelectList(_context.Customers.Where(c => c.RoleId == 4), "CustomerId", "CustomerName", contract.CustomerId);
            ViewData["PaymentMethodId"] = new SelectList(_context.PaymentMethods, "PaymentMethodId", "MethodName", contract.PaymentMethodId);
            ViewData["PriceId"] = new SelectList(_context.ServicePrices, "PriceId", "PriceId", contract.PriceId);
            ViewData["ServiceId"] = new SelectList(contractServices, "ServiceId", "ServiceName", contract.ServiceId);
            return PartialView("_Edit", contract);
        }

        // POST: AdminQL/Contracts/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ContractId,ContractCode,CustomerId,ServiceId,PriceId,DurationUnit,Duration,TotalAmount,StartDate,EndDate,PaymentMethodId,PaymentStatus,Status")] Contract contract)
        {
            if (id != contract.ContractId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(contract);
                    await _context.SaveChangesAsync();
                    // Thêm thông báo thành công vào TempData
                    TempData["SuccessMessage"] = "Cập nhật hợp đồng thành công!";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ContractExists(contract.ContractId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }

            var contractServices = _context.Services
                .Where(s => validContractServiceIds.Contains(s.ServiceId))
                .ToList();
            ViewData["CustomerId"] = new SelectList(_context.Customers.Where(c => c.RoleId == 4), "CustomerId", "CustomerName", contract.CustomerId);
            ViewData["PaymentMethodId"] = new SelectList(_context.PaymentMethods, "PaymentMethodId", "MethodName", contract.PaymentMethodId);
            ViewData["PriceId"] = new SelectList(_context.ServicePrices, "PriceId", "PriceId", contract.PriceId);
            ViewData["ServiceId"] = new SelectList(contractServices, "ServiceId", "ServiceName", contract.ServiceId);
            return PartialView("_Edit", contract);
        }

        // GET: AdminQL/Contracts/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var contract = await _context.Contracts
                .Include(c => c.Customer)
                .Include(c => c.PaymentMethod)
                .Include(c => c.Price)
                .Include(c => c.Service)
                .FirstOrDefaultAsync(m => m.ContractId == id);
            if (contract == null)
            {
                return NotFound();
            }

            return PartialView("_Delete", contract);
        }

        // POST: AdminQL/Contracts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var contract = await _context.Contracts.FindAsync(id);
            if (contract != null)
            {
                _context.Contracts.Remove(contract);
            }

            await _context.SaveChangesAsync();
            // Thêm thông báo thành công vào TempData
            TempData["SuccessMessage"] = "Xóa hợp đồng thành công!";
            return RedirectToAction(nameof(Index));
        }

        // GET: AdminQL/Contracts/PrintInvoice/5
        public async Task<IActionResult> PrintInvoice(int? id)
        {
            if (id == null)
            {
                return NotFound("Không có ID khách hàng được cung cấp.");
            }

            var contracts = await _context.Contracts
                .Include(c => c.Customer)
                .Include(c => c.PaymentMethod)
                .Include(c => c.Price)
                .Include(c => c.Service)
                    .ThenInclude(c => c.ServiceDescriptions)
                .Include(c => c.CareSchedules)
                .Where(s => s.ContractId == id)
                .ToListAsync();

            if (contracts == null || !contracts.Any())
            {
                return NotFound("Không tìm thấy hợp đồng nào cho khách hàng này.");
            }

            return View(contracts);
        }
        public async Task GenerateCareSchedules(Contract contract, DayOfWeek? careDay1 = null, DayOfWeek? careDay2 = null)
        {
            // Nếu không chỉ định ngày, mặc định là thứ 2 và thứ 5
            DayOfWeek day1 = careDay1 ?? DayOfWeek.Monday;
            DayOfWeek day2 = careDay2 ?? DayOfWeek.Thursday;

            // Xác định ngày bắt đầu và kết thúc của hợp đồng
            if (!contract.StartDate.HasValue || !contract.EndDate.HasValue)
            {
                throw new ArgumentException("StartDate và EndDate của hợp đồng không được để trống.");
            }

            DateTime startDate = contract.StartDate.Value;
            DateTime endDate = contract.EndDate.Value;

            // Lặp qua từng tuần trong khoảng thời gian hợp đồng
            DateTime currentDate = startDate;
            while (currentDate <= endDate)
            {
                // Tìm ngày đầu tiên của tuần (thứ 2)
                DateTime monday = currentDate.AddDays(-(int)currentDate.DayOfWeek + (int)DayOfWeek.Monday);
                if (monday < startDate) monday = startDate;

                // Tính ngày chăm sóc 1
                DateTime careDate1 = monday.AddDays((int)day1 - (int)DayOfWeek.Monday);
                if (careDate1 < startDate) careDate1 = startDate;

                // Tạo lịch chăm sóc cho ngày 1
                if (careDate1 <= endDate)
                {
                    var careSchedule1 = new CareSchedule
                    {
                        ContractId = contract.ContractId,
                        ScheduledDate = DateOnly.FromDateTime(careDate1),
                        ScheduledTime = new TimeOnly(9, 0), // 9:00 AM
                        Duration = 2.0m, // 2 giờ
                        Status = "Chưa thực hiện"
                    };
                    _context.CareSchedules.Add(careSchedule1);
                }

                // Tính ngày chăm sóc 2
                DateTime careDate2 = monday.AddDays((int)day2 - (int)DayOfWeek.Monday);
                if (careDate2 < startDate) careDate2 = startDate;

                // Tạo lịch chăm sóc cho ngày 2
                if (careDate2 <= endDate)
                {
                    var careSchedule2 = new CareSchedule
                    {
                        ContractId = contract.ContractId,
                        ScheduledDate = DateOnly.FromDateTime(careDate2),
                        ScheduledTime = new TimeOnly(9, 0), // 9:00 AM
                        Duration = 2.0m, // 2 giờ
                        Status = "Chưa thực hiện"
                    };
                    _context.CareSchedules.Add(careSchedule2);
                }

                // Chuyển sang tuần tiếp theo
                currentDate = monday.AddDays(7);
            }

            await _context.SaveChangesAsync();
        }

        private bool ContractExists(int id)
        {
            return _context.Contracts.Any(e => e.ContractId == id);
        }
    }
}