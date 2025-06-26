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
    public class ServiceCategoriesController : BaseController
    {
        private readonly OfficePlantCareContext _context;

        public ServiceCategoriesController(OfficePlantCareContext context)
        {
            _context = context;
        }

        // GET: AdminQL/ServiceCategories
        public async Task<IActionResult> Index(string name, int page = 1)
        {
            // Số ghi trên 1 trang
            int limit = 5;
            TempData["CurrentPage"] = page;

            // Tạo query cơ bản
            IQueryable<ServiceCategory> query = _context.ServiceCategories.OrderBy(c => c.CategoryServiceName);

            // Nếu có tham số name trên URL, thêm điều kiện lọc
            if (!string.IsNullOrEmpty(name))
            {
                query = query.Where(c => c.CategoryServiceName.Contains(name));
            }

            // Chuyển query sang danh sách
            var categories = await query.ToListAsync(); // Dùng ToListAsync() của EF Core

            // Sử dụng ToPagedList để phân trang (không bất đồng bộ)
            var pagedCategoy = categories.ToPagedList(page, limit);

            // Gửi từ khóa tìm kiếm cho View qua ViewBag
            ViewBag.keyword = name;

            return View(pagedCategoy);
        }

        // GET: AdminQL/ServiceCategories/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var serviceCategory = await _context.ServiceCategories.FindAsync(id);
            if (serviceCategory == null)
            {
                return NotFound();
            }
            return PartialView("_Details", serviceCategory);
        }

        // GET: AdminQL/ServiceCategories/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: AdminQL/ServiceCategories/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CategoryServiceId,CategoryServiceName")] ServiceCategory serviceCategory)
        {
            if (ModelState.IsValid)
            {
                _context.Add(serviceCategory);
                await _context.SaveChangesAsync();
                // Thêm thông báo thành công vào TempData
                TempData["SuccessMessage"] = "Thêm danh mục dịch vụ thành công!";
                int currentPage = TempData["CurrentPage"] != null ? (int)TempData["CurrentPage"] : 1;
                return RedirectToAction(nameof(Index), new { page = currentPage });
            }
            return View(serviceCategory);
        }

        // GET: AdminQL/ServiceCategories/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var serviceCategory = await _context.ServiceCategories.FindAsync(id);
            if (serviceCategory == null)
            {
                return NotFound();
            }
            return PartialView("_Edit", serviceCategory);
        }

        // POST: AdminQL/ServiceCategories/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("CategoryServiceId,CategoryServiceName")] ServiceCategory serviceCategory)
        {
            if (id != serviceCategory.CategoryServiceId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(serviceCategory);
                    await _context.SaveChangesAsync();
                    // Thêm thông báo thành công vào TempData
                    TempData["SuccessMessage"] = "Cập nhật danh mục dịch vụ thành công!";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ServiceCategoryExists(serviceCategory.CategoryServiceId))
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
            return View(serviceCategory);
        }

        // GET: AdminQL/ServiceCategories/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var serviceCategory = await _context.ServiceCategories.FindAsync(id);
            if (serviceCategory == null)
            {
                return NotFound();
            }
            return PartialView("_Delete", serviceCategory);
        }

        // POST: AdminQL/ServiceCategories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var serviceCategory = await _context.ServiceCategories.FindAsync(id);
            if (serviceCategory != null)
            {
                _context.ServiceCategories.Remove(serviceCategory);
            }

            await _context.SaveChangesAsync();
            // Thêm thông báo thành công vào TempData
            TempData["SuccessMessage"] = "Xóa danh mục dịch vụ thành công!";
            int currentPage = TempData["CurrentPage"] != null ? (int)TempData["CurrentPage"] : 1;
            return RedirectToAction(nameof(Index), new { page = currentPage });
        }

        private bool ServiceCategoryExists(int id)
        {
            return _context.ServiceCategories.Any(e => e.CategoryServiceId == id);
        }
    }
}
