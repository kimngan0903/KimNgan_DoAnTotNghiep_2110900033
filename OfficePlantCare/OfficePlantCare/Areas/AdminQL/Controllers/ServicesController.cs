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
    public class ServicesController : BaseController
    {
        private readonly OfficePlantCareContext _context;

        public ServicesController(OfficePlantCareContext context)
        {
            _context = context;
        }

        // GET: AdminQL/Services
        public async Task<IActionResult> Index(string name, int page = 1)
        {
            // Số ghi trên 1 trang
            int limit = 5;

            // Tạo query cơ bản
            IQueryable<Service> query = _context.Services.OrderBy(c => c.ServiceName);

            // Nếu có tham số name trên URL, thêm điều kiện lọc
            if (!string.IsNullOrEmpty(name))
            {
                query = query.Where(c => c.ServiceName.Contains(name));
            }

            // Chuyển query sang danh sách
            var service = await query.ToListAsync(); // Dùng ToListAsync() của EF Core

            // Sử dụng ToPagedList để phân trang (không bất đồng bộ)
            var pagedService = service.ToPagedList(page, limit);

            // Gửi từ khóa tìm kiếm cho View qua ViewBag
            ViewBag.keyword = name;

            return View(pagedService);
        }

        // GET: AdminQL/Services/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var service = await _context.Services
                .Include(s => s.CategoryService)
                .FirstOrDefaultAsync(m => m.ServiceId == id);
            if (service == null)
            {
                return NotFound();
            }

            return PartialView("_Details", service);
        }

        // GET: AdminQL/Services/Create
        public IActionResult Create()
        {
            ViewData["CategoryServiceId"] = new SelectList(_context.ServiceCategories, "CategoryServiceId", "CategoryServiceId");
            return View();
        }

        // POST: AdminQL/Services/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ServiceId,ServiceName,Description,CreatedDate,UpdatedDate,Status,CategoryServiceId,Image")] Service service)
        {
            if (ModelState.IsValid)
            {
                _context.Add(service);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoryServiceId"] = new SelectList(_context.ServiceCategories, "CategoryServiceId", "CategoryServiceId", service.CategoryServiceId);
            return View(service);
        }

        // GET: AdminQL/Services/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var service = await _context.Services.FindAsync(id);
            if (service == null)
            {
                return NotFound();
            }
            ViewData["CategoryServiceId"] = new SelectList(_context.ServiceCategories, "CategoryServiceId", "CategoryServiceId", service.CategoryServiceId);
            return PartialView("_Edit", service);
        }

        // POST: AdminQL/Services/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ServiceId,ServiceName,Description,CreatedDate,UpdatedDate,Status,CategoryServiceId,Image")] Service service)
        {
            if (id != service.ServiceId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(service);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ServiceExists(service.ServiceId))
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
            ViewData["CategoryServiceId"] = new SelectList(_context.ServiceCategories, "CategoryServiceId", "CategoryServiceId", service.CategoryServiceId);
            return View(service);
        }

        // GET: AdminQL/Services/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var service = await _context.Services
                .Include(s => s.CategoryService)
                .FirstOrDefaultAsync(m => m.ServiceId == id);
            if (service == null)
            {
                return NotFound();
            }

            return PartialView("_Delete", service);
        }

        // POST: AdminQL/Services/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var service = await _context.Services.FindAsync(id);
            if (service != null)
            {
                _context.Services.Remove(service);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ServiceExists(int id)
        {
            return _context.Services.Any(e => e.ServiceId == id);
        }
    }
}
