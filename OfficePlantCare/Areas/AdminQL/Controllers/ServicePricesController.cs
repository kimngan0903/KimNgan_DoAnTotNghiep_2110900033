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
    public class ServicePricesController : BaseController
    {
        private readonly OfficePlantCareContext _context;

        public ServicePricesController(OfficePlantCareContext context)
        {
            _context = context;
        }

        // GET: AdminQL/ServicePrices
        //public async Task<IActionResult> Index()
        //{
        //    var officePlantCareContext = _context.ServicePrices.Include(s => s.Service);
        //    return View(await officePlantCareContext.ToListAsync());
        //}
        public async Task<IActionResult> Index(string name, int page = 1)
        {
            // Số ghi trên 1 trang
            int limit = 5;
            TempData["CurrentPage"] = page;

            // Tạo query cơ bản
            IQueryable<ServicePrice> query = _context.ServicePrices
                                        .Include(s => s.Service)
                                        .OrderBy(c => c.ServiceType);

            // Nếu có tham số name trên URL, thêm điều kiện lọc
            if (!string.IsNullOrEmpty(name))
            {
                query = query.Where(c => c.ServiceType.Contains(name));
            }

            // Chuyển query sang danh sách
            var price = await query.ToListAsync(); // Dùng ToListAsync() của EF Core

            // Sử dụng ToPagedList để phân trang (không bất đồng bộ)
            var pagedPrice = price.ToPagedList(page, limit);

            // Gửi từ khóa tìm kiếm cho View qua ViewBag
            ViewBag.keyword = name;

            return View(pagedPrice);
        }
        // GET: AdminQL/ServicePrices/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var servicePrice = await _context.ServicePrices
                .Include(s => s.Service)
                .FirstOrDefaultAsync(m => m.PriceId == id);
            if (servicePrice == null)
            {
                return NotFound();
            }

            return PartialView("_Details", servicePrice);
        }

        // GET: AdminQL/ServicePrices/Create
        public IActionResult Create()
        {
            ViewData["ServiceId"] = new SelectList(_context.Services, "ServiceId", "ServiceId");
            return View();
        }

        // POST: AdminQL/ServicePrices/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("PriceId,ServiceId,ServiceType,TreeSize,OfficeSize,DurationInMonths,NumberOfTrees,Price")] ServicePrice servicePrice)
        {
            if (ModelState.IsValid)
            {
                _context.Add(servicePrice);
                await _context.SaveChangesAsync();
                // Thêm thông báo thành công vào TempData
                TempData["SuccessMessage"] = "Thêm giá dịch vụ thành công!";
                int currentPage = TempData["CurrentPage"] != null ? (int)TempData["CurrentPage"] : 1;
                return RedirectToAction(nameof(Index), new { page = currentPage });
            }
            ViewData["ServiceId"] = new SelectList(_context.Services, "ServiceId", "ServiceId", servicePrice.ServiceId);
            return View(servicePrice);
        }

        // GET: AdminQL/ServicePrices/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var servicePrice = await _context.ServicePrices
                           .Include(s => s.Service)
                           .FirstOrDefaultAsync(m => m.PriceId == id);
            if (servicePrice == null)
            {
                return NotFound();
            }
            ViewData["ServiceName"] = servicePrice.Service?.ServiceName;
            ViewData["ServiceId"] = new SelectList(_context.Services, "ServiceId", "ServiceId", servicePrice.ServiceId);
            return PartialView("_Edit", servicePrice);
        }

        // POST: AdminQL/ServicePrices/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("PriceId,ServiceId,ServiceType,TreeSize,OfficeSize,DurationInMonths,NumberOfTrees,Price")] ServicePrice servicePrice)
        {
            if (id != servicePrice.PriceId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(servicePrice);
                    await _context.SaveChangesAsync();
                    // Thêm thông báo thành công vào TempData
                    TempData["SuccessMessage"] = "Cập nhật giá dịch vụ thành công!";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ServicePriceExists(servicePrice.PriceId))
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
            ViewData["ServiceId"] = new SelectList(_context.Services, "ServiceId", "ServiceId", servicePrice.ServiceId);
            return View(servicePrice);
        }

        // GET: AdminQL/ServicePrices/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var servicePrice = await _context.ServicePrices
                .Include(s => s.Service)
                .FirstOrDefaultAsync(m => m.PriceId == id);
            if (servicePrice == null)
            {
                return NotFound();
            }

            return PartialView("_Delete", servicePrice);
        }

        // POST: AdminQL/ServicePrices/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var servicePrice = await _context.ServicePrices.FindAsync(id);
            if (servicePrice != null)
            {
                _context.ServicePrices.Remove(servicePrice);
            }

            await _context.SaveChangesAsync();
            // Thêm thông báo thành công vào TempData
            TempData["SuccessMessage"] = "Xóa giá dịch vụ thành công!";
            int currentPage = TempData["CurrentPage"] != null ? (int)TempData["CurrentPage"] : 1;
            return RedirectToAction(nameof(Index), new { page = currentPage });
        }

        private bool ServicePriceExists(int id)
        {
            return _context.ServicePrices.Any(e => e.PriceId == id);
        }
    }
}
