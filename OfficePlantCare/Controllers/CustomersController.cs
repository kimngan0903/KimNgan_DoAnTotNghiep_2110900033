using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OfficePlantCare.Areas.AdminQL.Controllers;
using OfficePlantCare.Models;

namespace OfficePlantCare.Controllers
{
    public class CustomersController : Controller
    {
        private readonly OfficePlantCareContext _context;
        private readonly CareScheduleController _careSchedulesController;
        private readonly List<int> validContractServiceIds = new List<int> { 1, 2, 3, 6, 12, 14 };

        public CustomersController(OfficePlantCareContext context, CareScheduleController careSchedulesController)
        {
            _context = context;
            _careSchedulesController = careSchedulesController;
        }

        // GET: Customers
        public async Task<IActionResult> Index()
        {
            var officePlantCareContext = _context.Customers.Include(c => c.Role);
            return View(await officePlantCareContext.ToListAsync());
        }

        // GET: Customers/Details/5
        public async Task<IActionResult> Details(int? customerId)
        {
            if (customerId == null)
            {
                return NotFound();
            }

            var customer = await _context.Customers
                .Include(c => c.Role)
                .FirstOrDefaultAsync(m => m.CustomerId == customerId);
            if (customer == null)
            {
                return NotFound();
            }

            return View(customer);
        }

        // GET: Customers/Create
        public IActionResult Create()
        {
            ViewData["RoleId"] = new SelectList(_context.Roles, "RoleId", "RoleName");
            ViewBag.ValidContractServices = _context.Services
                .Where(s => validContractServiceIds.Contains(s.ServiceId))
                .ToList();
            ViewBag.ServicePrices = _context.ServicePrices
                .Where(sp => validContractServiceIds.Contains(sp.ServiceId ?? 0))
                .Select(sp => new
                {
                    sp.PriceId,
                    sp.ServiceId,
                    sp.OfficeSize,
                    sp.TreeSize,
                    sp.ServiceType,
                    sp.Price,
                    sp.DurationInMonths,
                    sp.NumberOfTrees
                })
                .ToList();
            return View();
        }

        // POST: Customers/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind("CustomerName,Email,Phone,Address,RoleId,PasswordHash,CreatedDate,Status")] Customer customer,
            int? ServiceId, string OfficeSize, string TreeSize, string ServiceType, int? PriceId, int? Duration,
            string CareDay1, string CareDay2)
        {
            // Kiểm tra email và số điện thoại trùng lặp
            var existingEmail = await _context.Customers
                .FirstOrDefaultAsync(c => c.Email == customer.Email);
            if (existingEmail != null)
            {
                ModelState.AddModelError("Email", "Email đã tồn tại. Vui lòng sử dụng email khác.");
            }

            var existingPhone = await _context.Customers
                .FirstOrDefaultAsync(c => c.Phone == customer.Phone);
            if (existingPhone != null)
            {
                ModelState.AddModelError("Phone", "Số điện thoại đã tồn tại. Vui lòng sử dụng số điện thoại khác.");
            }

            // Nếu RoleId != 4, bỏ qua validation cho các trường hợp đồng
            if (customer.RoleId != 4)
            {
                ModelState.Remove("CareDay1");
                ModelState.Remove("CareDay2");
                ModelState.Remove("OfficeSize");
                ModelState.Remove("TreeSize");
                ModelState.Remove("ServiceType");
            }
            else
            {
                // Chỉ validate các trường hợp đồng nếu RoleId = 4
                if (string.IsNullOrEmpty(OfficeSize))
                {
                    ModelState.AddModelError("OfficeSize", "Kích thước văn phòng là bắt buộc.");
                }
                if (string.IsNullOrEmpty(TreeSize))
                {
                    ModelState.AddModelError("TreeSize", "Kích thước cây là bắt buộc.");
                }
                if (string.IsNullOrEmpty(ServiceType))
                {
                    ModelState.AddModelError("ServiceType", "Thời hạn dịch vụ là bắt buộc.");
                }
            }

            if (!ModelState.IsValid)
            {
                ViewData["RoleId"] = new SelectList(_context.Roles, "RoleId", "RoleName", customer.RoleId);
                ViewBag.ValidContractServices = _context.Services
                    .Where(s => validContractServiceIds.Contains(s.ServiceId))
                    .ToList();
                ViewBag.ServicePrices = _context.ServicePrices
                    .Where(sp => validContractServiceIds.Contains(sp.ServiceId ?? 0))
                    .Select(sp => new
                    {
                        sp.PriceId,
                        sp.ServiceId,
                        sp.OfficeSize,
                        sp.TreeSize,
                        sp.ServiceType,
                        sp.Price,
                        sp.DurationInMonths,
                        sp.NumberOfTrees
                    })
                    .ToList();
                return View(customer);
            }

            try
            {
                if (customer.CreatedDate == null)
                {
                    customer.CreatedDate = DateTime.Now;
                }

                if (string.IsNullOrEmpty(customer.Status))
                {
                    customer.Status = "Hoạt động";
                }

                _context.Customers.Add(customer);
                await _context.SaveChangesAsync();

                if (customer.RoleId == 4)
                {
                    if (!ServiceId.HasValue || !PriceId.HasValue || !Duration.HasValue || string.IsNullOrEmpty(OfficeSize) || string.IsNullOrEmpty(TreeSize) || string.IsNullOrEmpty(ServiceType))
                    {
                        ModelState.AddModelError("", "Vui lòng chọn đầy đủ thông tin hợp đồng (Dịch vụ, Kích thước văn phòng, Kích thước cây, Thời hạn dịch vụ).");
                        ViewData["RoleId"] = new SelectList(_context.Roles, "RoleId", "RoleName", customer.RoleId);
                        ViewBag.ValidContractServices = _context.Services
                            .Where(s => validContractServiceIds.Contains(s.ServiceId))
                            .ToList();
                        ViewBag.ServicePrices = _context.ServicePrices
                            .Where(sp => validContractServiceIds.Contains(sp.ServiceId ?? 0))
                            .Select(sp => new
                            {
                                sp.PriceId,
                                sp.ServiceId,
                                sp.OfficeSize,
                                sp.TreeSize,
                                sp.ServiceType,
                                sp.Price,
                                sp.DurationInMonths,
                                sp.NumberOfTrees
                            })
                            .ToList();
                        return View(customer);
                    }

                    if (!validContractServiceIds.Contains(ServiceId.Value))
                    {
                        ModelState.AddModelError("", "Dịch vụ được chọn không hợp lệ cho hợp đồng.");
                        ViewData["RoleId"] = new SelectList(_context.Roles, "RoleId", "RoleName", customer.RoleId);
                        ViewBag.ValidContractServices = _context.Services
                            .Where(s => validContractServiceIds.Contains(s.ServiceId))
                            .ToList();
                        ViewBag.ServicePrices = _context.ServicePrices
                            .Where(sp => validContractServiceIds.Contains(sp.ServiceId ?? 0))
                            .Select(sp => new
                            {
                                sp.PriceId,
                                sp.ServiceId,
                                sp.OfficeSize,
                                sp.TreeSize,
                                sp.ServiceType,
                                sp.Price,
                                sp.DurationInMonths,
                                sp.NumberOfTrees
                            })
                            .ToList();
                        return View(customer);
                    }

                    var selectedPrice = await _context.ServicePrices
                        .FirstOrDefaultAsync(sp => sp.PriceId == PriceId);

                    if (selectedPrice == null)
                    {
                        ModelState.AddModelError("", "Không tìm thấy giá dịch vụ phù hợp để tạo hợp đồng.");
                        ViewData["RoleId"] = new SelectList(_context.Roles, "RoleId", "RoleName", customer.RoleId);
                        ViewBag.ValidContractServices = _context.Services
                            .Where(s => validContractServiceIds.Contains(s.ServiceId))
                            .ToList();
                        ViewBag.ServicePrices = _context.ServicePrices
                            .Where(sp => validContractServiceIds.Contains(sp.ServiceId ?? 0))
                            .Select(sp => new
                            {
                                sp.PriceId,
                                sp.ServiceId,
                                sp.OfficeSize,
                                sp.TreeSize,
                                sp.ServiceType,
                                sp.Price,
                                sp.DurationInMonths,
                                sp.NumberOfTrees
                            })
                            .ToList();
                        return View(customer);
                    }

                    string contractCode = GenerateContractCode();

                    var contract = new Contract
                    {
                        ContractCode = contractCode,
                        CustomerId = customer.CustomerId,
                        ServiceId = ServiceId.Value,
                        PriceId = PriceId.Value,
                        DurationUnit = "Tháng",
                        Duration = Duration.Value,
                        Status = "Chờ xử lý",
                        PaymentStatus = "Chưa thanh toán",
                        TotalAmount = selectedPrice.Price,
                        StartDate = DateTime.Now,
                        EndDate = DateTime.Now.AddMonths(Duration.Value)
                    };

                    _context.Contracts.Add(contract);
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
                }

                TempData["SuccessMessage"] = "Đăng ký thành công! Lịch chăm sóc đã được tạo.";
                return RedirectToAction("Index", "Login");
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null)
                {
                    ModelState.AddModelError("", "Chi tiết lỗi: " + ex.InnerException.Message);
                }
                else
                {
                    ModelState.AddModelError("", "Đã xảy ra lỗi: " + ex.Message);
                }
            }

            ViewData["RoleId"] = new SelectList(_context.Roles, "RoleId", "RoleName", customer.RoleId);
            ViewBag.ValidContractServices = _context.Services
                .Where(s => validContractServiceIds.Contains(s.ServiceId))
                .ToList();
            ViewBag.ServicePrices = _context.ServicePrices
                .Where(sp => validContractServiceIds.Contains(sp.ServiceId ?? 0))
                .Select(sp => new
                {
                    sp.PriceId,
                    sp.ServiceId,
                    sp.OfficeSize,
                    sp.TreeSize,
                    sp.ServiceType,
                    sp.Price,
                    sp.DurationInMonths,
                    sp.NumberOfTrees
                })
                .ToList();
            return View(customer);
        }
        private string GenerateContractCode()
        {
            string prefix = "CT-";
            string datePart = DateTime.Now.ToString("yyyyMMdd");
            var lastContract = _context.Contracts
                .OrderByDescending(c => c.ContractId)
                .FirstOrDefault();

            int sequence = lastContract != null ? int.Parse(lastContract.ContractCode.Substring(11)) + 1 : 1;
            string sequencePart = sequence.ToString("D3");

            return $"{prefix}{datePart}-{sequencePart}";
        }

        // GET: Customers/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var customer = await _context.Customers.FindAsync(id);
            if (customer == null)
            {
                return NotFound();
            }
            ViewData["RoleId"] = new SelectList(_context.Roles, "RoleId", "RoleName", customer.RoleId);
            return View(customer);
        }

        // POST: Customers/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("CustomerId,CustomerName,Email,Phone,Address,CustomerType,RoleId,PasswordHash,CreatedDate,Status")] Customer customer)
        {
            if (id != customer.CustomerId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var existingCustomer = await _context.Customers.FindAsync(id);
                    if (existingCustomer == null)
                    {
                        return NotFound();
                    }

                    customer.RoleId = existingCustomer.RoleId;
                    _context.Entry(existingCustomer).CurrentValues.SetValues(customer);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CustomerExists(customer.CustomerId))
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
            ViewData["RoleId"] = new SelectList(_context.Roles, "RoleId", "RoleName", customer.RoleId);
            return View(customer);
        }

        // GET: Customers/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var customer = await _context.Customers
                .Include(c => c.Role)
                .FirstOrDefaultAsync(m => m.CustomerId == id);
            if (customer == null)
            {
                return NotFound();
            }

            return View(customer);
        }

        // POST: Customers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var customer = await _context.Customers.FindAsync(id);
            if (customer != null)
            {
                _context.Customers.Remove(customer);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
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

        private bool CustomerExists(int id)
        {
            return _context.Customers.Any(e => e.CustomerId == id);
        }
    }
}