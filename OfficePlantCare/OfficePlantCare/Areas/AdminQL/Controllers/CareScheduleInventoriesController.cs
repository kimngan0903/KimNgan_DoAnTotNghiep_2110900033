using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OfficePlantCare.Models;

namespace OfficePlantCare.Areas.AdminQL.Controllers
{
    public class CareScheduleInventoriesController : BaseController
    {
        private readonly OfficePlantCareContext _context;

        public CareScheduleInventoriesController(OfficePlantCareContext context)
        {
            _context = context;
        }

        // GET: AdminQL/CareScheduleInventories
        public async Task<IActionResult> Index()
        {
            var officePlantCareContext = _context.CareScheduleInventories.Include(c => c.CareSchedule).Include(c => c.Item);
            return View(await officePlantCareContext.ToListAsync());
        }

        // GET: AdminQL/CareScheduleInventories/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var careScheduleInventory = await _context.CareScheduleInventories
                .Include(c => c.CareSchedule)
                .Include(c => c.Item)
                .FirstOrDefaultAsync(m => m.CareScheduleInventoryId == id);
            if (careScheduleInventory == null)
            {
                return NotFound();
            }

            return View(careScheduleInventory);
        }

        // GET: AdminQL/CareScheduleInventories/Create
        public IActionResult Create()
        {
            ViewData["CareScheduleId"] = new SelectList(_context.CareSchedules, "ScheduleId", "ScheduleId");
            ViewData["ItemId"] = new SelectList(_context.InventoryItems, "ItemId", "ItemId");
            return View();
        }

        // POST: AdminQL/CareScheduleInventories/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CareScheduleInventoryId,CareScheduleId,ItemId,QuantityUsed,Notes")] CareScheduleInventory careScheduleInventory)
        {
            if (ModelState.IsValid)
            {
                _context.Add(careScheduleInventory);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CareScheduleId"] = new SelectList(_context.CareSchedules, "ScheduleId", "ScheduleId", careScheduleInventory.CareScheduleId);
            ViewData["ItemId"] = new SelectList(_context.InventoryItems, "ItemId", "ItemId", careScheduleInventory.ItemId);
            return View(careScheduleInventory);
        }

        // GET: AdminQL/CareScheduleInventories/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var careScheduleInventory = await _context.CareScheduleInventories.FindAsync(id);
            if (careScheduleInventory == null)
            {
                return NotFound();
            }
            ViewData["CareScheduleId"] = new SelectList(_context.CareSchedules, "ScheduleId", "ScheduleId", careScheduleInventory.CareScheduleId);
            ViewData["ItemId"] = new SelectList(_context.InventoryItems, "ItemId", "ItemId", careScheduleInventory.ItemId);
            return View(careScheduleInventory);
        }

        // POST: AdminQL/CareScheduleInventories/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("CareScheduleInventoryId,CareScheduleId,ItemId,QuantityUsed,Notes")] CareScheduleInventory careScheduleInventory)
        {
            if (id != careScheduleInventory.CareScheduleInventoryId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(careScheduleInventory);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CareScheduleInventoryExists(careScheduleInventory.CareScheduleInventoryId))
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
            ViewData["CareScheduleId"] = new SelectList(_context.CareSchedules, "ScheduleId", "ScheduleId", careScheduleInventory.CareScheduleId);
            ViewData["ItemId"] = new SelectList(_context.InventoryItems, "ItemId", "ItemId", careScheduleInventory.ItemId);
            return View(careScheduleInventory);
        }

        // GET: AdminQL/CareScheduleInventories/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var careScheduleInventory = await _context.CareScheduleInventories
                .Include(c => c.CareSchedule)
                .Include(c => c.Item)
                .FirstOrDefaultAsync(m => m.CareScheduleInventoryId == id);
            if (careScheduleInventory == null)
            {
                return NotFound();
            }

            return View(careScheduleInventory);
        }

        // POST: AdminQL/CareScheduleInventories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var careScheduleInventory = await _context.CareScheduleInventories.FindAsync(id);
            if (careScheduleInventory != null)
            {
                _context.CareScheduleInventories.Remove(careScheduleInventory);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CareScheduleInventoryExists(int id)
        {
            return _context.CareScheduleInventories.Any(e => e.CareScheduleInventoryId == id);
        }
    }
}
