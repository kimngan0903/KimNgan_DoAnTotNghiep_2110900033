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
    public class CareScheduleSuppliesController : BaseController
    {
        private readonly OfficePlantCareContext _context;

        public CareScheduleSuppliesController(OfficePlantCareContext context)
        {
            _context = context;
        }

        // GET: AdminQL/CareScheduleSupplies
        public async Task<IActionResult> Index(string name, int page = 1)
        {
            // Số ghi trên 1 trang
            int limit = 5;

            // Tạo query cơ bản
            IQueryable<CareScheduleSupply> query = _context.CareScheduleSupplies
                                              .Include(c => c.Schedule)
                                              .Include(c => c.Supply)
                                              .OrderBy(c => c.Notes);
            // Nếu có tham số name trên URL, thêm điều kiện lọc
            if (!string.IsNullOrEmpty(name))
            {
                query = query.Where(c => c.Notes.Contains(name));
            }

            // Chuyển query sang danh sách
            var careScheduleSupply = await query.ToListAsync(); // Dùng ToListAsync() của EF Core

            // Sử dụng ToPagedList để phân trang (không bất đồng bộ)
            var pagedCareScheduleSupply = careScheduleSupply.ToPagedList(page, limit);

            // Gửi từ khóa tìm kiếm cho View qua ViewBag
            ViewBag.keyword = name;
            return View(pagedCareScheduleSupply);
        }
        // GET: AdminQL/CareScheduleSupplies/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var careScheduleSupply = await _context.CareScheduleSupplies
                .Include(c => c.Schedule)
                .Include(c => c.Supply)
                .FirstOrDefaultAsync(m => m.CareScheduleSupplyId == id);
            if (careScheduleSupply == null)
            {
                return NotFound();
            }

            return PartialView("_Details", careScheduleSupply);
        }

        // GET: AdminQL/CareScheduleSupplies/Create
        public IActionResult Create()
        {
            ViewData["ScheduleId"] = new SelectList(_context.CareSchedules, "ScheduleId", "ScheduleId");
            ViewData["SupplyId"] = new SelectList(_context.Supplies, "SupplyId", "SupplyName");
            return View();
        }

        // POST: AdminQL/CareScheduleSupplies/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CareScheduleSupplyId,ScheduleId,SupplyId,QuantityUsed,Notes")] CareScheduleSupply careScheduleSupply)
        {
            if (ModelState.IsValid)
            {
                _context.Add(careScheduleSupply);
                await _context.SaveChangesAsync();
                // Thêm thông báo thành công vào TempData
                TempData["SuccessMessage"] = "Thêm vật tư lịch chăm sóc thành công!";
                return RedirectToAction(nameof(Index));
            }
            ViewData["ScheduleId"] = new SelectList(_context.CareSchedules, "ScheduleId", "ScheduleId", careScheduleSupply.ScheduleId);
            ViewData["SupplyId"] = new SelectList(_context.Supplies, "SupplyId", "SupplyName", careScheduleSupply.SupplyId);
            return View(careScheduleSupply);
        }

        // GET: AdminQL/CareScheduleSupplies/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var careScheduleSupply = await _context.CareScheduleSupplies.FindAsync(id);
            if (careScheduleSupply == null)
            {
                return NotFound();
            }
            ViewData["ScheduleId"] = new SelectList(_context.CareSchedules, "ScheduleId", "ScheduleId", careScheduleSupply.ScheduleId);
            ViewData["SupplyId"] = new SelectList(_context.Supplies, "SupplyId", "SupplyName", careScheduleSupply.SupplyId);
            return PartialView("_Edit", careScheduleSupply);
        }

        // POST: AdminQL/CareScheduleSupplies/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("CareScheduleSupplyId,ScheduleId,SupplyId,QuantityUsed,Notes")] CareScheduleSupply careScheduleSupply)
        {
            if (id != careScheduleSupply.CareScheduleSupplyId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(careScheduleSupply);
                    await _context.SaveChangesAsync();
                    // Thêm thông báo thành công vào TempData
                    TempData["SuccessMessage"] = "Cập nhật vật tư lịch chăm sóc thành công!";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CareScheduleSupplyExists(careScheduleSupply.CareScheduleSupplyId))
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
            ViewData["ScheduleId"] = new SelectList(_context.CareSchedules, "ScheduleId", "ScheduleId", careScheduleSupply.ScheduleId);
            ViewData["SupplyId"] = new SelectList(_context.Supplies, "SupplyId", "SupplyName", careScheduleSupply.SupplyId);
            return View(careScheduleSupply);
        }

        // GET: AdminQL/CareScheduleSupplies/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var careScheduleSupply = await _context.CareScheduleSupplies
                .Include(c => c.Schedule)
                .Include(c => c.Supply)
                .FirstOrDefaultAsync(m => m.CareScheduleSupplyId == id);
            if (careScheduleSupply == null)
            {
                return NotFound();
            }

            return PartialView("_Delete", careScheduleSupply);
        }

        // POST: AdminQL/CareScheduleSupplies/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var careScheduleSupply = await _context.CareScheduleSupplies.FindAsync(id);
            if (careScheduleSupply != null)
            {
                _context.CareScheduleSupplies.Remove(careScheduleSupply);
            }

            await _context.SaveChangesAsync();
            // Thêm thông báo thành công vào TempData
            TempData["SuccessMessage"] = "Xóa vật tư lịch chăm sóc thành công!";
            return RedirectToAction(nameof(Index));
        }

        private bool CareScheduleSupplyExists(int id)
        {
            return _context.CareScheduleSupplies.Any(e => e.CareScheduleSupplyId == id);
        }
    }
}
