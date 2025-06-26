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
    public class OrderDetailsController : BaseController
    {
        private readonly OfficePlantCareContext _context;

        public OrderDetailsController(OfficePlantCareContext context)
        {
            _context = context;
        }

        // GET: AdminQL/OrderDetails
        public async Task<IActionResult> Index(string name, int page = 1)
        {
            // Số ghi trên 1 trang
            int limit = 5;
            TempData["CurrentPage"] = page;

            // Tạo query cơ bản
            IQueryable<OrderDetail> query = _context.OrderDetails
                                            .Include(o => o.Order)
                                            .Include(o => o.Price)
                                            .Include(o => o.Service)
                                            .OrderBy(c => c.Address);

            // Nếu có tham số name trên URL, thêm điều kiện lọc
            if (!string.IsNullOrEmpty(name))
            {
                query = query.Where(c => c.Address.Contains(name));
            }

            // Chuyển query sang danh sách
            var orderDetails = await query.ToListAsync(); // Dùng ToListAsync() của EF Core

            // Sử dụng ToPagedList để phân trang (không bất đồng bộ)
            var pagedOrderDetails = orderDetails.ToPagedList(page, limit);

            // Gửi từ khóa tìm kiếm cho View qua ViewBag
            ViewBag.keyword = name;

            return View(pagedOrderDetails);
        }
        // GET: AdminQL/OrderDetails/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var orderDetail = await _context.OrderDetails
                .Include(o => o.Order)
                .Include(o => o.Price)
                .Include(o => o.Service)
                .FirstOrDefaultAsync(m => m.OrderDetailId == id);
            if (orderDetail == null)
            {
                return NotFound();
            }

            return PartialView("_Details", orderDetail);
        }

        // GET: AdminQL/OrderDetails/Create
        public IActionResult Create()
        {
            ViewData["OrderId"] = new SelectList(_context.Orders, "OrderId", "OrderId");
            ViewData["PriceId"] = new SelectList(_context.ServicePrices, "PriceId", "PriceId");
            ViewData["ServiceId"] = new SelectList(_context.Services, "ServiceId", "ServiceName");
            return View();
        }

        // POST: AdminQL/OrderDetails/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("OrderDetailId,OrderId,ServiceId,Address,Quantity,PriceId,TotalAmount,Notes,Status")] OrderDetail orderDetail)
        {
            if (ModelState.IsValid)
            {
                _context.Add(orderDetail);
                await _context.SaveChangesAsync();
                // Thêm thông báo thành công vào TempData
                TempData["SuccessMessage"] = "Thêm chi tiết đơn hàng thành công!";
                int currentPage = TempData["CurrentPage"] != null ? (int)TempData["CurrentPage"] : 1;
                return RedirectToAction(nameof(Index), new { page = currentPage });
            }
            ViewData["OrderId"] = new SelectList(_context.Orders, "OrderId", "OrderId", orderDetail.OrderId);
            ViewData["PriceId"] = new SelectList(_context.ServicePrices, "PriceId", "PriceId", orderDetail.PriceId);
            ViewData["ServiceId"] = new SelectList(_context.Services, "ServiceId", "ServiceName", orderDetail.ServiceId);
            return View(orderDetail);
        }

        // GET: AdminQL/OrderDetails/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var orderDetail = await _context.OrderDetails
                        .Include(o => o.Order)
                        .Include(o => o.Price)
                        .Include(o => o.Service)
                        .FirstOrDefaultAsync(m => m.OrderDetailId == id);

            if (orderDetail == null)
            {
                return NotFound();
            }
            ViewData["ServiceName"] = orderDetail.Service?.ServiceName;
            ViewData["OrderName"] = orderDetail.Order?.Customer?.CustomerName;
            ViewData["OrderId"] = new SelectList(_context.Orders, "OrderId", "OrderId", orderDetail.OrderId);
            ViewData["PriceId"] = new SelectList(_context.ServicePrices, "PriceId", "PriceId", orderDetail.PriceId);
            //ViewData["ServiceId"] = new SelectList(_context.Services, "ServiceId", "ServiceName", orderDetail.ServiceId);
            return PartialView("_Edit", orderDetail);
        }

        // POST: AdminQL/OrderDetails/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("OrderDetailId,OrderId,ServiceId,Address,Quantity,PriceId,TotalAmount,Notes,Status")] OrderDetail orderDetail)
        {
            if (id != orderDetail.OrderDetailId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(orderDetail);
                    await _context.SaveChangesAsync();
                    // Thêm thông báo thành công vào TempData
                    TempData["SuccessMessage"] = "Cập nhật chi tiết đơn hàng thành công!";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OrderDetailExists(orderDetail.OrderDetailId))
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
            ViewData["OrderId"] = new SelectList(_context.Orders, "OrderId", "OrderId", orderDetail.OrderId);
            ViewData["PriceId"] = new SelectList(_context.ServicePrices, "PriceId", "PriceId", orderDetail.PriceId);
            ViewData["ServiceId"] = new SelectList(_context.Services, "ServiceId", "ServiceName", orderDetail.ServiceId);
            return View(orderDetail);
        }

        // GET: AdminQL/OrderDetails/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var orderDetail = await _context.OrderDetails
                .Include(o => o.Order)
                .Include(o => o.Price)
                .Include(o => o.Service)
                .FirstOrDefaultAsync(m => m.OrderDetailId == id);
            if (orderDetail == null)
            {
                return NotFound();
            }

            return PartialView("_Delete", orderDetail);
        }

        // POST: AdminQL/OrderDetails/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var orderDetail = await _context.OrderDetails.FindAsync(id);
            if (orderDetail != null)
            {
                _context.OrderDetails.Remove(orderDetail);
            }

            await _context.SaveChangesAsync();
            // Thêm thông báo thành công vào TempData
            TempData["SuccessMessage"] = "Xóa chi tiết đơn hàng thành công!";
            int currentPage = TempData["CurrentPage"] != null ? (int)TempData["CurrentPage"] : 1;
            return RedirectToAction(nameof(Index), new { page = currentPage });
        }

        private bool OrderDetailExists(int id)
        {
            return _context.OrderDetails.Any(e => e.OrderDetailId == id);
        }
    }
}
