using System;
using System.Linq;
using System.Threading.Tasks;
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
        private readonly CareSchedulesController _careSchedulesController;

        public ContractsController(OfficePlantCareContext context, CareSchedulesController careSchedulesController)
        {
            _context = context;
            _careSchedulesController = careSchedulesController;
        }

    // GET: AdminQL/Contracts
    public async Task<IActionResult> Index(string name, int page = 1, int? confirmPayment = null)
        {
            int limit = 5;
            TempData["CurrentPage"] = page;

            IQueryable<Contract> query = _context.Contracts
                .Include(c => c.Customer)
                .Include(c => c.ContractDetails)
                    .ThenInclude(cd => cd.Price)
                .Include(c => c.PaymentMethod)
                .Include(c => c.ContractDetails)
                    .ThenInclude(cd => cd.Service)
                .OrderBy(c => c.Customer.CustomerName);

            if (!string.IsNullOrEmpty(name))
            {
                query = query.Where(c => c.Customer.CustomerName.Contains(name));
            }

            // Xử lý xác nhận thanh toán
            if (confirmPayment.HasValue)
            {
                var contract = await _context.Contracts
                    .Where(s => s.CustomerId == confirmPayment.Value)
                    .ToListAsync();

                if (contract.Any())
                {
                    foreach (var contracts in contract)
                    {
                        if (contracts.PaymentStatus == "Chưa thanh toán")
                        {
                            contracts.PaymentStatus = "Đã thanh toán";
                            _context.Update(contracts);
                        }
                    }
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Xác nhận thanh toán thành công cho hợp đồng của bạn!";
                }
                else
                {
                    TempData["ErrorMessage"] = "Không tìm thấy yêu cầu nào để cập nhật cho khách hàng này.";
                }
            }

            var contractList = await query.ToListAsync();
            var pagedContract = contractList.ToPagedList(page, limit);

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
                .Include(c => c.ContractDetails)
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
            var today = DateTime.Today;
            var contractsTodayCount = _context.Contracts
                .Count(c => c.CreatedDate.HasValue && c.CreatedDate.Value.Date == today);
            string contractCode = $"HD-{today:yyyyMMdd}-{contractsTodayCount + 1:D3}";
            ViewBag.ContractCode = contractCode;

            ViewBag.CustomerId = new SelectList(_context.Customers, "CustomerId", "CustomerName");
            ViewBag.PaymentMethodId = new SelectList(_context.PaymentMethods, "PaymentMethodId", "MethodName");

            // Get services with "tháng" in ServiceType
            var monthlyServices = _context.ServicePrices
                .Where(sp => sp.ServiceType != null && sp.ServiceType.Contains("tháng"))
                .Select(sp => sp.ServiceId)
                .Distinct()
                .ToList();

            var services = _context.Services
                .Where(s => monthlyServices.Contains(s.ServiceId))
                .ToList();

            ViewBag.ServiceId = new SelectList(services, "ServiceId", "ServiceName");

            ViewBag.ServicePrices = _context.ServicePrices
                .Select(p => new
                {
                    p.PriceId,
                    p.ServiceId,
                    p.OfficeSize,
                    p.TreeSize,
                    p.ServiceType,
                    p.Price,
                    DurationInMonths = GetDurationInMonths(p.ServiceType)
                })
                .ToList();

            return View();
        }

        // Helper method to safely parse DurationInMonths
        private static int GetDurationInMonths(string serviceType)
        {
            if (string.IsNullOrEmpty(serviceType))
                return 0;

            var parts = serviceType.Split(' ');
            if (parts.Length > 0 && int.TryParse(parts[0], out int months))
                return months;

            return 0;
        }

        // POST: AdminQL/Contracts/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Contract contract, int serviceId, string officeSize, string treeSize, string serviceType, int priceId, string careDay1, string careDay2)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Set default values
                    contract.CreatedDate = DateTime.Now;
                    contract.DurationUnit = "Tháng";
                    contract.Status = "Chờ xử lý";

                    // Validate required fields
                    if (contract.CustomerId == 0)
                    {
                        ModelState.AddModelError("CustomerId", "Vui lòng chọn khách hàng.");
                        return View(contract);
                    }
                    if (contract.StartDate == default(DateOnly)) // Check for invalid StartDate
                    {
                        ModelState.AddModelError("StartDate", "Vui lòng chọn ngày bắt đầu.");
                        return View(contract);
                    }
                    if (!contract.PaymentMethodId.HasValue || contract.PaymentMethodId == 0)
                    {
                        ModelState.AddModelError("PaymentMethodId", "Vui lòng chọn phương thức thanh toán.");
                        return View(contract);
                    }

                    // Calculate EndDate based on Duration
                    if (!string.IsNullOrEmpty(serviceType))
                    {
                        var durationMonths = GetDurationInMonths(serviceType);
                        if (durationMonths > 0)
                        {
                            var startDate = contract.StartDate.ToDateTime(new TimeOnly(0, 0));
                            var endDate = startDate.AddMonths(durationMonths);
                            contract.EndDate = DateOnly.FromDateTime(endDate);
                        }
                        else
                        {
                            ModelState.AddModelError("ServiceType", "Thời hạn dịch vụ không hợp lệ. Vui lòng chọn lại.");
                            return View(contract);
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("ServiceType", "Vui lòng chọn thời hạn dịch vụ.");
                        return View(contract);
                    }

                    // Set Duration from serviceType
                    contract.Duration = GetDurationInMonths(serviceType);

                    _context.Add(contract);
                    await _context.SaveChangesAsync();

                    TempData["SuccessMessage"] = "Thêm mới hợp đồng thành công!";

                    var contractDetail = new ContractDetail
                    {
                        ContractId = contract.ContractId,
                        ServiceId = serviceId,
                        Address = (await _context.Customers.FindAsync(contract.CustomerId))?.Address,
                        PriceId = priceId,
                        TotalAmount = contract.TotalAmount ?? 0m, // Ensure TotalAmount is not null
                        Status = "Đang chờ xác nhận"
                    };
                    _context.ContractDetails.Add(contractDetail);

                    // Create care schedules
                    int numberOfSchedules = contract.Duration * 4 * 2; // 4 weeks/month, 2 times/week
                    var scheduleStartDate = contract.StartDate.ToDateTime(new TimeOnly(0, 0));
                    var daysOfWeek = new List<string> { "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday", "Sunday" };

                    if (string.IsNullOrEmpty(careDay1) || string.IsNullOrEmpty(careDay2))
                    {
                        var random = new Random();
                        var selectedDays = daysOfWeek.OrderBy(x => random.Next()).Take(2).ToList();
                        careDay1 = selectedDays[0];
                        careDay2 = selectedDays[1];
                    }

                    for (int i = 0; i < numberOfSchedules; i++)
                    {
                        var baseDate = scheduleStartDate.AddDays((i / 2) * 7);
                        var dayOfWeek = (i % 2 == 0) ? careDay1 : careDay2;
                        var daysUntilTargetDay = ((int)Enum.Parse(typeof(DayOfWeek), dayOfWeek) - (int)baseDate.DayOfWeek + 7) % 7;
                        var scheduledDate = baseDate.AddDays(daysUntilTargetDay);

                        var careSchedule = new CareSchedule
                        {
                            ContractId = contract.ContractId,
                            OrderId = null,
                            StaffId = null,
                            ScheduledDate = DateOnly.FromDateTime(scheduledDate),
                            ScheduledTime = new TimeOnly(9, 0),
                            ActualDate = null,
                            Duration = 2.0m,
                            Status = "Chưa thực hiện"
                        };
                        _context.CareSchedules.Add(careSchedule);
                    }

                    await _context.SaveChangesAsync();

                    TempData["SuccessMessage"] = "Tạo hợp đồng thành công!";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    TempData["ErrorMessage"] = $"Có lỗi xảy ra khi tạo hợp đồng: {ex.Message}";
                    return RedirectToAction(nameof(Create));
                }
            }

            ViewBag.CustomerId = new SelectList(_context.Customers, "CustomerId", "CustomerName", contract.CustomerId);
            ViewBag.PaymentMethodId = new SelectList(_context.PaymentMethods, "PaymentMethodId", "MethodName", contract.PaymentMethodId);
            ViewBag.ServiceId = new SelectList(_context.Services, "ServiceId", "ServiceName", serviceId);
            ViewBag.ServicePrices = _context.ServicePrices
                .Select(p => new
                {
                    p.PriceId,
                    p.ServiceId,
                    p.OfficeSize,
                    p.TreeSize,
                    p.ServiceType,
                    p.Price,
                    DurationInMonths = GetDurationInMonths(p.ServiceType)
                })
                .ToList();

            return View(contract);
        }

        // GET: AdminQL/Contracts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var contract = await _context.Contracts
                                         .Include(cd => cd.Customer)
                                         .Include(cd => cd.PaymentMethod)
                                         .Include(cd => cd.ContractDetails)
                                         .FirstOrDefaultAsync(o => o.ContractId == id);
            if (contract == null)
            {
                return NotFound();
            }
            ViewData["CustomerName"] = contract.Customer?.CustomerName;
            ViewData["CustomerId"] = new SelectList(_context.Customers, "CustomerId", "CustomerName", contract.CustomerId);
            ViewData["PaymentMethodId"] = new SelectList(_context.PaymentMethods, "PaymentMethodId", "MethodName", contract.PaymentMethodId);
            return PartialView("_Edit", contract);
        }

        // POST: AdminQL/Contracts/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ContractId,ContractCode,CustomerId,CreatedDate,TotalAmount,DurationUnit,Duration,StartDate,EndDate,PaymentMethodId,PaymentStatus,Status")] Contract contract)
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
                    TempData["SuccessMessage"] = "Chỉnh sửa hợp đồng thành công!";
                    // Gọi phương thức để cập nhật trạng thái CareSchedule
                    bool updated = await _careSchedulesController.UpdateCareScheduleStatusForContract(contract.ContractId);
                    if (updated)
                    {
                        TempData["SuccessMessage"] = "Trạng thái chi tiết đơn hàng đã được cập nhật.";
                    }
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

            ViewData["CustomerId"] = new SelectList(_context.Customers, "CustomerId", "CustomerName", contract.CustomerId);
            ViewData["PaymentMethodId"] = new SelectList(_context.PaymentMethods, "PaymentMethodId", "MethodName", contract.PaymentMethodId);
            return View(contract);
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
            var contract = await _context.Contracts
                .Include(c => c.ContractDetails) // Include related ContractDetails
                .FirstOrDefaultAsync(c => c.ContractId == id);

            if (contract != null)
            {
                // Remove associated ContractDetails
                _context.ContractDetails.RemoveRange(contract.ContractDetails);
                _context.Contracts.Remove(contract);
            }

            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Xóa hợp đồng thành công!";
            return RedirectToAction(nameof(Index));
        }

        // GET: AdminQL/Contracts/PrintInvoice/5
        public async Task<IActionResult> PrintInvoice(int? id)
        {
            if (id == null)
            {
                return NotFound("Không có ID hợp đồng được cung cấp.");
            }

            var contracts = await _context.Contracts
                .Include(c => c.Customer)
                .Include(c => c.PaymentMethod)
                .Include(c => c.ContractDetails)
                    .ThenInclude(cd => cd.Price)
                .Include(c => c.ContractDetails)
                    .ThenInclude(cd => cd.Service)
                .Include(c => c.CareSchedules)
                .Where(s => s.ContractId == id)
                .ToListAsync();

            if (contracts == null || !contracts.Any())
            {
                return NotFound("Không tìm thấy hợp đồng nào cho ID này.");
            }

            return View(contracts);
        }

        private bool ContractExists(int id)
        {
            return _context.Contracts.Any(e => e.ContractId == id);
        }
    }
}
