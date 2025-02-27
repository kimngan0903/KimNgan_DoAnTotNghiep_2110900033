using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OfficePlantCare.Models;
using X.PagedList.Extensions;

namespace OfficePlantCare.Areas.AdminQL.Controllers
{
    public class CustomersController : BaseController
    {
        private readonly OfficePlantCareContext _context;
        private readonly List<int> validContractServiceIds = new List<int> { 1, 3, 10, 12, 14 };
        public CustomersController(OfficePlantCareContext context)
        {
            _context = context;
        }

        // GET: AdminQL/Customers
        public async Task<IActionResult> Index(string name, int page = 1)
        {
            // Số ghi trên 1 trang
            int limit = 5;

            // Tạo query cơ bản
            IQueryable<Customer> query = _context.Customers.OrderBy(c => c.CustomerName);

            // Nếu có tham số name trên URL, thêm điều kiện lọc
            if (!string.IsNullOrEmpty(name))
            {
                query = query.Where(c => c.CustomerName.Contains(name));
            }

            // Chuyển query sang danh sách
            var customer = await query.ToListAsync(); // Dùng ToListAsync() của EF Core

            // Sử dụng ToPagedList để phân trang (không bất đồng bộ)
            var pagedCustomer = customer.ToPagedList(page, limit);

            // Gửi từ khóa tìm kiếm cho View qua ViewBag
            ViewBag.keyword = name;

            return View(pagedCustomer);
        }

        // GET: AdminQL/Customers/Details/5
        public async Task<IActionResult> Details(int? id)
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

            return PartialView("_Details", customer);
        }

        // GET: AdminQL/Customers/Create
        // GET: AdminQL/Customers/Create
        public IActionResult Create()
        {
            ViewData["RoleId"] = new SelectList(_context.Roles, "RoleId", "RoleName");
            ViewData["PaymentMethodId"] = new SelectList(_context.PaymentMethods, "PaymentMethodId", "MethodName");

            // Hiển thị tất cả dịch vụ nếu là RoleId = 4
            ViewData["ServiceId"] = new SelectList(_context.Services, "ServiceId", "ServiceName");

            return View();
        }


        // POST: AdminQL/Customers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        // POST: AdminQL/Customers/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CustomerId,CustomerName,Email,Phone,Address,CustomerType,RoleId,PasswordHash,CreatedDate,Status")] Customer customer, int? ServiceId, decimal FixedPrice, string DurationUnit, int Duration, int? PaymentMethodId)
        {
            if (ModelState.IsValid)
            {
                // Lưu khách hàng vào database
                _context.Add(customer);
                await _context.SaveChangesAsync();

                // Nếu là RoleId = 4 (khách ký hợp đồng)
                if (customer.RoleId == 4 && ServiceId.HasValue)
                {
                    if (validContractServiceIds.Contains(ServiceId.Value))
                    {
                        // Nếu dịch vụ nằm trong hợp đồng, tạo hợp đồng mới
                        var contract = new Contract
                        {
                            CustomerId = customer.CustomerId,
                            ServiceId = ServiceId.Value,
                            FixedPrice = FixedPrice,
                            DurationUnit = DurationUnit,
                            Duration = Duration,
                            StartDate = DateOnly.FromDateTime(DateTime.Now),
                            EndDate = DurationUnit == "Tháng"
                                ? DateOnly.FromDateTime(DateTime.Now.AddMonths(Duration))
                                : DateOnly.FromDateTime(DateTime.Now.AddYears(Duration)),
                            PaymentMethodId = PaymentMethodId,
                            TotalAmount = FixedPrice * Duration,
                            PaidAmount = 0,
                            RemainingAmount = FixedPrice * Duration,
                            Status = "Đang hiệu lực",
                            CreatedAt = DateTime.Now,
                            UpdatedAt = DateTime.Now
                        };

                        _context.Add(contract);
                    }
                    else
                    {
                        // Nếu dịch vụ là phát sinh, lưu vào ServiceRequest
                        var serviceRequest = new ServiceRequest
                        {
                            CustomerId = customer.CustomerId,
                            ServiceId = ServiceId.Value,
                            RequestDate = DateTime.Now,
                            //Status = "Chờ xử lý",
                            //Price = FixedPrice,
                            //Quantity = 1, // Mặc định là 1, có thể cập nhật theo yêu cầu
                            //TotalAmount = FixedPrice * 1, // Tính tổng giá
                            UpdatedAt = DateTime.Now
                        };

                        _context.Add(serviceRequest);
                    }

                    await _context.SaveChangesAsync();
                }

                return RedirectToAction(nameof(Index));
            }

            ViewData["RoleId"] = new SelectList(_context.Roles, "RoleId", "RoleName", customer.RoleId);
            ViewData["ServiceId"] = new SelectList(_context.Services, "ServiceId", "ServiceName", ServiceId);
            ViewData["PaymentMethodId"] = new SelectList(_context.PaymentMethods, "PaymentMethodId", "MethodName", PaymentMethodId);
            return View(customer);
        }


        // GET: AdminQL/Customers/Edit/5
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
            ViewData["RoleId"] = new SelectList(_context.Roles, "RoleId", "RoleId", customer.RoleId);
            return PartialView("_Edit", customer);
        }

        // POST: AdminQL/Customers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
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
                    _context.Update(customer);
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
            ViewData["RoleId"] = new SelectList(_context.Roles, "RoleId", "RoleId", customer.RoleId);
            return View(customer);
        }

        // GET: AdminQL/Customers/Delete/5
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

            return PartialView("_Delete", customer);
        }

        // POST: AdminQL/Customers/Delete/5
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

        private bool CustomerExists(int id)
        {
            return _context.Customers.Any(e => e.CustomerId == id);
        }
    }
}
