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
    public class ServiceInventoriesController : BaseController
    {
        private readonly OfficePlantCareContext _context;

        public ServiceInventoriesController(OfficePlantCareContext context)
        {
            _context = context;
        }

        // GET: AdminQL/ServiceInventories
        public async Task<IActionResult> Index()
        {
            var officePlantCareContext = _context.ServiceInventories.Include(s => s.Item).Include(s => s.Service);
            return View(await officePlantCareContext.ToListAsync());
        }

        // GET: AdminQL/ServiceInventories/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var serviceInventory = await _context.ServiceInventories
                .Include(s => s.Item)
                .Include(s => s.Service)
                .FirstOrDefaultAsync(m => m.ServiceInventoryId == id);
            if (serviceInventory == null)
            {
                return NotFound();
            }

            return View(serviceInventory);
        }

        // GET: AdminQL/ServiceInventories/Create
        public IActionResult Create()
        {
            ViewData["ItemId"] = new SelectList(_context.InventoryItems, "ItemId", "ItemId");
            ViewData["ServiceId"] = new SelectList(_context.Services, "ServiceId", "ServiceId");
            return View();
        }

        // POST: AdminQL/ServiceInventories/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ServiceInventoryId,ServiceId,ItemId,QuantityRequired,Notes")] ServiceInventory serviceInventory)
        {
            if (ModelState.IsValid)
            {
                _context.Add(serviceInventory);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ItemId"] = new SelectList(_context.InventoryItems, "ItemId", "ItemId", serviceInventory.ItemId);
            ViewData["ServiceId"] = new SelectList(_context.Services, "ServiceId", "ServiceId", serviceInventory.ServiceId);
            return View(serviceInventory);
        }

        // GET: AdminQL/ServiceInventories/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var serviceInventory = await _context.ServiceInventories.FindAsync(id);
            if (serviceInventory == null)
            {
                return NotFound();
            }
            ViewData["ItemId"] = new SelectList(_context.InventoryItems, "ItemId", "ItemId", serviceInventory.ItemId);
            ViewData["ServiceId"] = new SelectList(_context.Services, "ServiceId", "ServiceId", serviceInventory.ServiceId);
            return View(serviceInventory);
        }

        // POST: AdminQL/ServiceInventories/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ServiceInventoryId,ServiceId,ItemId,QuantityRequired,Notes")] ServiceInventory serviceInventory)
        {
            if (id != serviceInventory.ServiceInventoryId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(serviceInventory);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ServiceInventoryExists(serviceInventory.ServiceInventoryId))
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
            ViewData["ItemId"] = new SelectList(_context.InventoryItems, "ItemId", "ItemId", serviceInventory.ItemId);
            ViewData["ServiceId"] = new SelectList(_context.Services, "ServiceId", "ServiceId", serviceInventory.ServiceId);
            return View(serviceInventory);
        }

        // GET: AdminQL/ServiceInventories/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var serviceInventory = await _context.ServiceInventories
                .Include(s => s.Item)
                .Include(s => s.Service)
                .FirstOrDefaultAsync(m => m.ServiceInventoryId == id);
            if (serviceInventory == null)
            {
                return NotFound();
            }

            return View(serviceInventory);
        }

        // POST: AdminQL/ServiceInventories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var serviceInventory = await _context.ServiceInventories.FindAsync(id);
            if (serviceInventory != null)
            {
                _context.ServiceInventories.Remove(serviceInventory);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ServiceInventoryExists(int id)
        {
            return _context.ServiceInventories.Any(e => e.ServiceInventoryId == id);
        }
    }
}
