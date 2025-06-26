using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OfficePlantCare.Models;
using X.PagedList.Extensions;

namespace OfficePlantCare.Areas.AdminQL.Controllers
{
    public class OrdersController : BaseController
    {
        private readonly OfficePlantCareContext _context;
        private readonly CareSchedulesController _careSchedulesController;

        public OrdersController(OfficePlantCareContext context, CareSchedulesController careSchedulesController)
        {
            _context = context;
            _careSchedulesController = careSchedulesController;
        }

        // GET: AdminQL/Orders
        public async Task<IActionResult> Index(string name, int page = 1, int? confirmPayment = null)
        {
            // Số ghi trên 1 trang
            int limit = 5;
            TempData["CurrentPage"] = page;

            // Tạo query cơ bản
            IQueryable<Order> query = _context.Orders
                                            .Include(o => o.Customer)
                                            .Include(o => o.PaymentMethod)
                                            .Include(o => o.Staff)
                                            .OrderBy(c => c.Customer.CustomerName);

            // Nếu có tham số name trên URL, thêm điều kiện lọc
            if (!string.IsNullOrEmpty(name))
            {
                query = query.Where(c => c.Customer.CustomerName.Contains(name));
            }
            // Xử lý xác nhận thanh toán
            if (confirmPayment.HasValue)
            {
                var order = await _context.Orders
                    .Where(s => s.CustomerId == confirmPayment.Value)
                    .ToListAsync();

                if (order.Any())
                {
                    foreach (var orders in order)
                    {
                        if (orders.PaymentStatus == "Chưa thanh toán")
                        {
                            orders.PaymentStatus = "Đã thanh toán";
                            orders.PaymentDate = DateTime.Now;
                            _context.Update(orders);
                        }
                    }
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Xác nhận thanh toán thành công cho đơn hàng của khách hàng!";
                }
                else
                {
                    TempData["ErrorMessage"] = "Không tìm thấy yêu cầu nào để cập nhật cho khách hàng này.";
                }
            }
            // Chuyển query sang danh sách
            var orderDetails = await query.ToListAsync(); // Dùng ToListAsync() của EF Core

            // Sử dụng ToPagedList để phân trang (không bất đồng bộ)
            var pagedOrderDetails = orderDetails.ToPagedList(page, limit);

            // Gửi từ khóa tìm kiếm cho View qua ViewBag
            ViewBag.keyword = name;

            return View(pagedOrderDetails);
        }
        // GET: AdminQL/Orders/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Orders
                .Include(o => o.Customer)
                .Include(o => o.PaymentMethod)
                .Include(o => o.Staff)
                .FirstOrDefaultAsync(m => m.OrderId == id);
            if (order == null)
            {
                return NotFound();
            }

            return PartialView("_Details", order);
        }
        public async Task<IActionResult> Detail(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Orders
                .Include(o => o.Customer)
                .Include(o => o.PaymentMethod)
                .Include(o => o.Staff)
                .FirstOrDefaultAsync(m => m.OrderId == id);
            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        // GET: AdminQL/Orders/Create
        public IActionResult Create()
        {
            ViewData["CustomerId"] = new SelectList(_context.Customers, "CustomerId", "CustomerName");
            ViewData["PaymentMethodId"] = new SelectList(_context.PaymentMethods, "PaymentMethodId", "MethodName");
            ViewData["StaffId"] = new SelectList(_context.Staffs.Where(s => s.Position == "Nhân viên chăm sóc cây"), "StaffId", "StaffName");
            return View();
        }

        // POST: AdminQL/Orders/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("OrderId,CustomerId,StaffId,OrderDate,TotalPrice,PaymentMethodId,PaymentDate,PaymentStatus,Status")] Order order)
        {
            if (ModelState.IsValid)
            {
                _context.Add(order);
                await _context.SaveChangesAsync();  
                // Thêm thông báo thành công vào TempData
                TempData["SuccessMessage"] = "Thêm đơn hàng thành công!";
                int currentPage = TempData["CurrentPage"] != null ? (int)TempData["CurrentPage"] : 1;
                return RedirectToAction(nameof(Index), new { page = currentPage });
            }
            ViewData["CustomerId"] = new SelectList(_context.Customers, "CustomerId", "CustomerName", order.CustomerId);
            ViewData["PaymentMethodId"] = new SelectList(_context.PaymentMethods, "PaymentMethodId", "MethodName", order.PaymentMethodId);
            ViewData["StaffId"] = new SelectList(_context.Staffs.Where(s => s.Position == "Nhân viên chăm sóc cây"), "StaffId", "StaffName", order.StaffId);
            return View(order);
        }

        // GET: AdminQL/Orders/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Orders
                .Include(o => o.Customer) // Bao gồm thông tin khách hàng
                .FirstOrDefaultAsync(o => o.OrderId == id);

            if (order == null)
            {
                return NotFound();
            }

            // Truyền thông tin khách hàng vào ViewData
            ViewData["CustomerName"] = order.Customer?.CustomerName; // Lấy tên khách hàng
            ViewData["PaymentMethodId"] = new SelectList(_context.PaymentMethods, "PaymentMethodId", "MethodName", order.PaymentMethodId);
            ViewData["StaffId"] = new SelectList(_context.Staffs.Where(s => s.Position == "Nhân viên chăm sóc cây"), "StaffId", "StaffName", order.StaffId);
            return PartialView("_Edit", order);
        }

        // POST: AdminQL/Orders/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("OrderId,CustomerId,StaffId,OrderDate,TotalPrice,PaymentMethodId,PaymentDate,PaymentStatus,Status")] Order order)
        {
            if (id != order.OrderId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(order);
                    await _context.SaveChangesAsync();
                    // Thêm thông báo thành công vào TempData
                    TempData["SuccessMessage"] = "Cập nhật đơn hàng thành công!";
                    // Gọi phương thức để cập nhật trạng thái CareSchedule
                    bool updated = await _careSchedulesController.UpdateCareScheduleStatusForOrder(order.OrderId);
                    if (updated)
                    {
                        TempData["SuccessMessage"] = "Trạng thái lịch chăm sóc đã được cập nhật.";
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OrderExists(order.OrderId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                int currentPage = TempData["CurrentPage"] != null ? (int)TempData["CurrentPage"] : 1;
                return RedirectToAction(nameof(Index), new { page = currentPage });
            }
            ViewData["CustomerId"] = new SelectList(_context.Customers, "CustomerId", "CustomerName", order.CustomerId);
            ViewData["PaymentMethodId"] = new SelectList(_context.PaymentMethods, "PaymentMethodId", "MethodName", order.PaymentMethodId);
            ViewData["StaffId"] = new SelectList(_context.Staffs.Where(s => s.Position == "Nhân viên chăm sóc cây"), "StaffId", "StaffName", order.StaffId);
            return View(order);
        }

        // GET: AdminQL/Orders/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Orders
                .Include(o => o.Customer)
                .Include(o => o.PaymentMethod)
                .Include(o => o.Staff)
                .FirstOrDefaultAsync(m => m.OrderId == id);
            if (order == null)
            {
                return NotFound();
            }

            return PartialView("_Delete", order);
        }

        // POST: AdminQL/Orders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order != null)
            {
                _context.Orders.Remove(order);
            }

            await _context.SaveChangesAsync();
            // Thêm thông báo thành công vào TempData
            TempData["SuccessMessage"] = "Xóa đơn hàng thành công!";
            int currentPage = TempData["CurrentPage"] != null ? (int)TempData["CurrentPage"] : 1;
            return RedirectToAction(nameof(Index), new { page = currentPage });
        }
        // GET: AdminQL/Orders/PrintInvoice/5
        public async Task<IActionResult> PrintInvoice(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Orders
                .Include(o => o.OrderDetails) // Lấy danh sách OrderDetails
                    .ThenInclude(od => od.Service) // Lấy thông tin Service từ OrderDetails
                .Include(o => o.Customer) // Lấy thông tin khách hàng
                .Include(o => o.PaymentMethod) // Lấy phương thức thanh toán
                .Include(o => o.Staff) // Lấy thông tin nhân viên xử lý
                .FirstOrDefaultAsync(o => o.OrderId == id); // Chỉ lấy một đơn hàng

            if (order == null)
            {
                return NotFound("Không tìm thấy yêu cầu dịch vụ nào cho khách hàng này.");
            }

            return View(order);
        }

        private bool OrderExists(int id)
        {
            return _context.Orders.Any(e => e.OrderId == id);
        }
    }
}
