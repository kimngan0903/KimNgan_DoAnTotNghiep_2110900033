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
    public class ServiceDescriptionsController : BaseController
    {
        private readonly OfficePlantCareContext _context;

        public ServiceDescriptionsController(OfficePlantCareContext context)
        {
            _context = context;
        }

        // GET: AdminQL/ServiceDescriptions
        public async Task<IActionResult> Index(string name, int page = 1)
        {
            // Số ghi trên 1 trang
            int limit = 5;
            TempData["CurrentPage"] = page;

            // Tạo query cơ bản
            IQueryable<ServiceDescription> query = _context.ServiceDescriptions
                .Include(s => s.Service)
                .OrderBy(c => c.Title);

            // Nếu có tham số name trên URL, thêm điều kiện lọc
            if (!string.IsNullOrEmpty(name))
            {
                query = query.Where(c => c.Title.Contains(name));
            }

            // Chuyển query sang danh sách
            var description = await query.ToListAsync(); // Dùng ToListAsync() của EF Core

            // Sử dụng ToPagedList để phân trang (không bất đồng bộ)
            var pagedDescription = description.ToPagedList(page, limit);

            // Gửi từ khóa tìm kiếm cho View qua ViewBag
            ViewBag.keyword = name;

            return View(pagedDescription);
        }

        // GET: AdminQL/ServiceDescriptions/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var serviceDescription = await _context.ServiceDescriptions
                .Include(s => s.Service)
                .FirstOrDefaultAsync(m => m.DescriptionId == id);
            if (serviceDescription == null)
            {
                return NotFound();
            }

            return PartialView("_Details", serviceDescription);
        }

        // GET: AdminQL/ServiceDescriptions/Create
        public IActionResult Create()
        {
            ViewData["ServiceId"] = new SelectList(_context.Services, "ServiceId", "ServiceName");
            return View();
        }

        // POST: AdminQL/ServiceDescriptions/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("DescriptionId,ServiceId,Image,StepNumber,Title,Content,CreatedDate,UpdatedDate")] ServiceDescription serviceDescription)
        {
            if (ModelState.IsValid)
            {
                var files = HttpContext.Request.Form.Files;
                if (files.Any() && files[0].Length > 0)
                {
                    var file = files[0];
                    var fileName = file.FileName;
                    var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\img\\description", fileName);

                    using (var stream = new FileStream(path, FileMode.Create))
                    {
                        file.CopyTo(stream);
                        // Lưu đường dẫn vào Admin
                        serviceDescription.Image = "/img/description/" + fileName;
                    }
                }
                _context.Add(serviceDescription);
                await _context.SaveChangesAsync();
                // Thêm thông báo thành công vào TempData
                TempData["SuccessMessage"] = "Thêm mô tả dịch vụ thành công!";
                int currentPage = TempData["CurrentPage"] != null ? (int)TempData["CurrentPage"] : 1;
                return RedirectToAction(nameof(Index), new { page = currentPage });
            }
            ViewData["ServiceId"] = new SelectList(_context.Services, "ServiceId", "ServiceName", serviceDescription.ServiceId);
            return View(serviceDescription);
        }

        // GET: AdminQL/ServiceDescriptions/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var serviceDescription = await _context.ServiceDescriptions
                          .Include(s => s.Service)
                          .FirstOrDefaultAsync(m => m.DescriptionId == id);
            if (serviceDescription == null)
            {
                return NotFound();
            }
            ViewData["ServiceName"] = serviceDescription.Service.ServiceName;
            ViewData["ServiceId"] = new SelectList(_context.Services, "ServiceId", "ServiceName", serviceDescription.ServiceId);
            return PartialView("_Edit", serviceDescription);
        }

        // POST: AdminQL/ServiceDescriptions/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("DescriptionId,ServiceId,Image,StepNumber,Title,Content,CreatedDate,UpdatedDate")] ServiceDescription serviceDescription)
        {
            if (id != serviceDescription.DescriptionId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var files = HttpContext.Request.Form.Files;
                    if (files.Any() && files[0].Length > 0)
                    {
                        var file = files[0];
                        var fileName = file.FileName;
                        var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\img\\description", fileName);

                        using (var stream = new FileStream(path, FileMode.Create))
                        {
                            file.CopyTo(stream);
                            // Lưu đường dẫn vào Admin
                            serviceDescription.Image = "/img/description/" + fileName;
                        }
                    }
                    _context.Update(serviceDescription);
                    await _context.SaveChangesAsync();
                    // Thêm thông báo thành công vào TempData
                    TempData["SuccessMessage"] = "Cập nhật mô tả dịch vụ thành công!";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ServiceDescriptionExists(serviceDescription.DescriptionId))
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
            ViewData["ServiceId"] = new SelectList(_context.Services, "ServiceId", "ServiceName", serviceDescription.ServiceId);
            return View(serviceDescription);
        }

        // GET: AdminQL/ServiceDescriptions/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var serviceDescription = await _context.ServiceDescriptions
                .Include(s => s.Service)
                .FirstOrDefaultAsync(m => m.DescriptionId == id);
            if (serviceDescription == null)
            {
                return NotFound();
            }

            return PartialView("_Delete", serviceDescription);
        }

        // POST: AdminQL/ServiceDescriptions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var serviceDescription = await _context.ServiceDescriptions.FindAsync(id);
            if (serviceDescription != null)
            {
                _context.ServiceDescriptions.Remove(serviceDescription);
            }

            await _context.SaveChangesAsync();
            // Thêm thông báo thành công vào TempData
            TempData["SuccessMessage"] = "Xóa mô tả dịch vụ thành công!";
            int currentPage = TempData["CurrentPage"] != null ? (int)TempData["CurrentPage"] : 1;
            return RedirectToAction(nameof(Index), new { page = currentPage });
        }

        private bool ServiceDescriptionExists(int id)
        {
            return _context.ServiceDescriptions.Any(e => e.DescriptionId == id);
        }
    }
}
