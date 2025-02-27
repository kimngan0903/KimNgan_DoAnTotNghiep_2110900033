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
    public class CareSchedulesController : Controller
    {
        private readonly OfficePlantCareContext _context;

        public CareSchedulesController(OfficePlantCareContext context)
        {
            _context = context;
        }

        // GET: CareSchedules
        public async Task<IActionResult> Index()
        {
            var officePlantCareContext = _context.CareSchedules.Include(c => c.Contract).Include(c => c.Staff);
            return View(await officePlantCareContext.ToListAsync());
        }

        // GET: CareSchedules/Details/5
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

        // GET: CareSchedules/Create
        public IActionResult Create(int contractId)
        {
            var contract = _context.Contracts.FirstOrDefault(c => c.ContractId == contractId);
            if (contract == null)
            {
                return NotFound();
            }

            var model = new CareSchedule
            {
                ContractId = contractId,
                //ScheduledDate = DateTime.Now, // Mặc định ngày hẹn là hôm nay
                Status = "Chờ xử lý" // Trạng thái mặc định
            };

            ViewData["ContractId"] = new SelectList(_context.Contracts, "ContractId", "ContractName", contractId);
            ViewData["StaffId"] = new SelectList(_context.Staffs, "StaffId", "StaffName");

            return View(model);
        }



        // POST: CareSchedules/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ContractId,StaffId,ActualDate,Duration,Status")] CareSchedule careSchedule)
        {
            if (ModelState.IsValid)
            {
                var contract = await _context.Contracts.FindAsync(careSchedule.ContractId);
                if (contract != null)
                {
                    //careSchedule.ScheduledDate = contract.CreatedAt; // Luôn lấy ngày từ hợp đồng
                }
                else
                {
                    ModelState.AddModelError("", "Hợp đồng không tồn tại.");
                    return View(careSchedule);
                }

                careSchedule.Status = "Chờ xử lý"; // Mặc định là "Chờ xử lý"

                _context.Add(careSchedule);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["ContractId"] = new SelectList(_context.Contracts, "ContractId", "ContractName", careSchedule.ContractId);
            ViewData["StaffId"] = new SelectList(_context.Staffs, "StaffId", "StaffName", careSchedule.StaffId);
            return View(careSchedule);
        }



        // GET: CareSchedules/Edit/5
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

        // POST: CareSchedules/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ScheduleId,ContractId,StaffId,ActualDate,Duration,Status")] CareSchedule careSchedule)
        {
            if (id != careSchedule.ScheduleId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var contract = await _context.Contracts.FindAsync(careSchedule.ContractId);
                    if (contract != null)
                    {
                        //careSchedule.ScheduledDate = contract.CreatedAt; // Cập nhật ngày đặt lịch nếu hợp đồng thay đổi
                    }
                    else
                    {
                        ModelState.AddModelError("", "Hợp đồng không tồn tại.");
                        return View(careSchedule);
                    }

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

            ViewData["ContractId"] = new SelectList(_context.Contracts, "ContractId", "ContractName", careSchedule.ContractId);
            ViewData["StaffId"] = new SelectList(_context.Staffs, "StaffId", "StaffName", careSchedule.StaffId);
            return View(careSchedule);
        }

        // GET: CareSchedules/Delete/5
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

        // POST: CareSchedules/Delete/5
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
