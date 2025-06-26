using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OfficePlantCare.Models;

namespace OfficePlantCare.Controllers
{
    public class OrdersController : Controller
    {
        private readonly OfficePlantCareContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public OrdersController(OfficePlantCareContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        // GET: Orders
        public async Task<IActionResult> Index()
        {
            int? customerId = HttpContext.Session.GetInt32("CustomerId");
            if (customerId == null)
            {
                return RedirectToAction("Index", "Login"); // Nếu chưa đăng nhập, chuyển hướng về trang đăng nhập
            }

            var orders = await _context.Orders
                .Where(o => o.CustomerId == customerId.Value) // Chỉ lấy đơn hàng của khách đang đăng nhập
                .Include(o => o.Customer)
                .Include(o => o.PaymentMethod)
                .Include(o => o.Staff)
                .ToListAsync();
            var categories = _context.ServiceCategories
                                         .Include(c => c.Services) // N?p luôn danh sách d?ch v? thu?c danh m?c ?ó
                                         .ToList();

            ViewData["ServiceCategories"] = categories; // Truy?n vào ViewData
            return View(orders);
        }

        // GET: Orders/Details/5
        [HttpGet]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var categories = _context.ServiceCategories
                                        .Include(c => c.Services) // N?p luôn danh sách d?ch v? thu?c danh m?c ?ó
                                        .ToList();

            ViewData["ServiceCategories"] = categories; // Truy?n vào ViewData
            var customerId = _httpContextAccessor.HttpContext.Session.GetInt32("CustomerId");
            if (!customerId.HasValue)
            {
                return RedirectToAction("Index", "Login");
            }

            var order = await _context.Orders
                     .Include(o => o.OrderDetails)
                         .ThenInclude(od => od.Service)
                     .Include(o => o.OrderDetails)
                         .ThenInclude(od => od.Price)
                     .Include(o => o.Customer)
                     .Include(o => o.PaymentMethod)
                     .Include(o => o.Staff)
                     .FirstOrDefaultAsync(o => o.OrderId == id && o.CustomerId == customerId.Value);
            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CancelOrder(int orderId)
        {
            var customerId = _httpContextAccessor.HttpContext.Session.GetInt32("CustomerId");
            if (!customerId.HasValue)
            {
                return RedirectToAction("Index", "Login");
            }

            var order = await _context.Orders
                .Include(o => o.CareSchedules)
                .FirstOrDefaultAsync(o => o.OrderId == orderId && o.CustomerId == customerId.Value);

            if (order == null)
            {
                return NotFound();
            }

            if (order.Status != "Chờ xử lý" || order.PaymentStatus == "Đã thanh toán")
            {
                TempData["ErrorMessage"] = "Không thể hủy đơn hàng này.";
                return RedirectToAction("Index");
            }

            order.Status = "Hủy";

            if (order.CareSchedules.Any())
            {
                _context.CareSchedules.RemoveRange(order.CareSchedules);
            }

            _context.Update(order);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Đơn hàng và các lịch chăm sóc đã được hủy thành công.";
            return RedirectToAction("Index");
        }
        // GET: Orders/Create
        public IActionResult Create()
        {
            ViewData["CustomerId"] = new SelectList(_context.Customers, "CustomerId", "CustomerId");
            ViewData["PaymentMethodId"] = new SelectList(_context.PaymentMethods, "PaymentMethodId", "PaymentMethodId");
            ViewData["StaffId"] = new SelectList(_context.Staffs, "StaffId", "StaffId");
            return View();
        }

        // POST: Orders/Create
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
                return RedirectToAction(nameof(Index));
            }
            ViewData["CustomerId"] = new SelectList(_context.Customers, "CustomerId", "CustomerId", order.CustomerId);
            ViewData["PaymentMethodId"] = new SelectList(_context.PaymentMethods, "PaymentMethodId", "PaymentMethodId", order.PaymentMethodId);
            ViewData["StaffId"] = new SelectList(_context.Staffs, "StaffId", "StaffId", order.StaffId);
            return View(order);
        }

        // GET: Orders/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Orders.FindAsync(id);
            if (order == null)
            {
                return NotFound();
            }
            ViewData["CustomerId"] = new SelectList(_context.Customers, "CustomerId", "CustomerId", order.CustomerId);
            ViewData["PaymentMethodId"] = new SelectList(_context.PaymentMethods, "PaymentMethodId", "PaymentMethodId", order.PaymentMethodId);
            ViewData["StaffId"] = new SelectList(_context.Staffs, "StaffId", "StaffId", order.StaffId);
            return View(order);
        }

        // POST: Orders/Edit/5
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
                return RedirectToAction(nameof(Index));
            }
            ViewData["CustomerId"] = new SelectList(_context.Customers, "CustomerId", "CustomerId", order.CustomerId);
            ViewData["PaymentMethodId"] = new SelectList(_context.PaymentMethods, "PaymentMethodId", "PaymentMethodId", order.PaymentMethodId);
            ViewData["StaffId"] = new SelectList(_context.Staffs, "StaffId", "StaffId", order.StaffId);
            return View(order);
        }

        // GET: Orders/Delete/5
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

            return View(order);
        }

        // POST: Orders/Delete/5
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
            return RedirectToAction(nameof(Index));
        }

        private bool OrderExists(int id)
        {
            return _context.Orders.Any(e => e.OrderId == id);
        }
    }
}
