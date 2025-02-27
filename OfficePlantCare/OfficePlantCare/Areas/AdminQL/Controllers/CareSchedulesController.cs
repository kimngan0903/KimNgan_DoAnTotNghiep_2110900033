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
    public class CareSchedulesController : BaseController
    {
        private readonly OfficePlantCareContext _context;

        public CareSchedulesController(OfficePlantCareContext context)
        {
            _context = context;
        }

        // GET: AdminQL/CareSchedules
        public async Task<IActionResult> Index()
        {
            var officePlantCareContext = _context.CareSchedules.Include(c => c.Contract).Include(c => c.Staff);
            return View(await officePlantCareContext.ToListAsync());
        }

        // GET: AdminQL/CareSchedules/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var careSchedule = await _context.CareSchedules
                .Include(c => c.Contract)
                .Include(c => c.Staff)
                .FirstOrDefaultAsync(m => m.ScheduleId == id);
            if (careSchedule == null)
            {
                return NotFound();
            }

            return View(careSchedule);
        }

        // GET: AdminQL/CareSchedules/Create
        public IActionResult Create()
        {
            ViewData["ContractId"] = new SelectList(_context.Contracts, "ContractId", "ContractId");
            ViewData["StaffId"] = new SelectList(_context.Staffs, "StaffId", "StaffId");
            return View();
        }

        // POST: AdminQL/CareSchedules/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ScheduleId,ContractId,StaffId,ScheduledDate,ActualDate,Duration,Status")] CareSchedule careSchedule)
        {
            if (ModelState.IsValid)
            {
                _context.Add(careSchedule);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ContractId"] = new SelectList(_context.Contracts, "ContractId", "ContractId", careSchedule.ContractId);
            ViewData["StaffId"] = new SelectList(_context.Staffs, "StaffId", "StaffId", careSchedule.StaffId);
            return View(careSchedule);
        }

        // GET: AdminQL/CareSchedules/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var careSchedule = await _context.CareSchedules.FindAsync(id);
            if (careSchedule == null)
            {
                return NotFound();
            }
            ViewData["ContractId"] = new SelectList(_context.Contracts, "ContractId", "ContractId", careSchedule.ContractId);
            ViewData["StaffId"] = new SelectList(_context.Staffs, "StaffId", "StaffId", careSchedule.StaffId);
            return View(careSchedule);
        }

        // POST: AdminQL/CareSchedules/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ScheduleId,ContractId,StaffId,ScheduledDate,ActualDate,Duration,Status")] CareSchedule careSchedule)
        {
            if (id != careSchedule.ScheduleId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(careSchedule);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CareScheduleExists(careSchedule.ScheduleId))
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
            ViewData["ContractId"] = new SelectList(_context.Contracts, "ContractId", "ContractId", careSchedule.ContractId);
            ViewData["StaffId"] = new SelectList(_context.Staffs, "StaffId", "StaffId", careSchedule.StaffId);
            return View(careSchedule);
        }

        // GET: AdminQL/CareSchedules/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var careSchedule = await _context.CareSchedules
                .Include(c => c.Contract)
                .Include(c => c.Staff)
                .FirstOrDefaultAsync(m => m.ScheduleId == id);
            if (careSchedule == null)
            {
                return NotFound();
            }

            return View(careSchedule);
        }

        // POST: AdminQL/CareSchedules/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var careSchedule = await _context.CareSchedules.FindAsync(id);
            if (careSchedule != null)
            {
                _context.CareSchedules.Remove(careSchedule);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CareScheduleExists(int id)
        {
            return _context.CareSchedules.Any(e => e.ScheduleId == id);
        }
    }
}
