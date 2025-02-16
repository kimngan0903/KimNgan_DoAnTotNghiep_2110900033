using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PlantCare.Models;

namespace PlantCare.Areas.AdminQL.Controllers
{
    public class ServiceRequestsController : BaseController
    {
        private readonly OfficePlantCareContext _context;

        public ServiceRequestsController(OfficePlantCareContext context)
        {
            _context = context;
        }

        // GET: AdminQL/ServiceRequests
        public async Task<IActionResult> Index()
        {
            var officePlantCareContext = _context.ServiceRequests.Include(s => s.Customer).Include(s => s.ProcessedByNavigation).Include(s => s.Schedule).Include(s => s.Service);
            return View(await officePlantCareContext.ToListAsync());
        }

        // GET: AdminQL/ServiceRequests/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var serviceRequest = await _context.ServiceRequests
                .Include(s => s.Customer)
                .Include(s => s.ProcessedByNavigation)
                .Include(s => s.Schedule)
                .Include(s => s.Service)
                .FirstOrDefaultAsync(m => m.RequestId == id);
            if (serviceRequest == null)
            {
                return NotFound();
            }

            return View(serviceRequest);
        }

        // GET: AdminQL/ServiceRequests/Create
        public IActionResult Create()
        {
            ViewData["CustomerId"] = new SelectList(_context.Customers, "CustomerId", "CustomerId");
            ViewData["ProcessedBy"] = new SelectList(_context.Admins, "AdminId", "AdminId");
            ViewData["ScheduleId"] = new SelectList(_context.CareSchedules, "ScheduleId", "ScheduleId");
            ViewData["ServiceId"] = new SelectList(_context.Services, "ServiceId", "ServiceId");
            return View();
        }

        // POST: AdminQL/ServiceRequests/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("RequestId,ScheduleId,ServiceId,CustomerId,ProcessedBy,Quantity,Price,TotalAmount,Notes,RequestDate,UpdatedAt,Status")] ServiceRequest serviceRequest)
        {
            if (ModelState.IsValid)
            {
                _context.Add(serviceRequest);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CustomerId"] = new SelectList(_context.Customers, "CustomerId", "CustomerId", serviceRequest.CustomerId);
            ViewData["ProcessedBy"] = new SelectList(_context.Admins, "AdminId", "AdminId", serviceRequest.ProcessedBy);
            ViewData["ScheduleId"] = new SelectList(_context.CareSchedules, "ScheduleId", "ScheduleId", serviceRequest.ScheduleId);
            ViewData["ServiceId"] = new SelectList(_context.Services, "ServiceId", "ServiceId", serviceRequest.ServiceId);
            return View(serviceRequest);
        }

        // GET: AdminQL/ServiceRequests/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var serviceRequest = await _context.ServiceRequests.FindAsync(id);
            if (serviceRequest == null)
            {
                return NotFound();
            }
            ViewData["CustomerId"] = new SelectList(_context.Customers, "CustomerId", "CustomerId", serviceRequest.CustomerId);
            ViewData["ProcessedBy"] = new SelectList(_context.Admins, "AdminId", "AdminId", serviceRequest.ProcessedBy);
            ViewData["ScheduleId"] = new SelectList(_context.CareSchedules, "ScheduleId", "ScheduleId", serviceRequest.ScheduleId);
            ViewData["ServiceId"] = new SelectList(_context.Services, "ServiceId", "ServiceId", serviceRequest.ServiceId);
            return View(serviceRequest);
        }

        // POST: AdminQL/ServiceRequests/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("RequestId,ScheduleId,ServiceId,CustomerId,ProcessedBy,Quantity,Price,TotalAmount,Notes,RequestDate,UpdatedAt,Status")] ServiceRequest serviceRequest)
        {
            if (id != serviceRequest.RequestId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(serviceRequest);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ServiceRequestExists(serviceRequest.RequestId))
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
            ViewData["CustomerId"] = new SelectList(_context.Customers, "CustomerId", "CustomerId", serviceRequest.CustomerId);
            ViewData["ProcessedBy"] = new SelectList(_context.Admins, "AdminId", "AdminId", serviceRequest.ProcessedBy);
            ViewData["ScheduleId"] = new SelectList(_context.CareSchedules, "ScheduleId", "ScheduleId", serviceRequest.ScheduleId);
            ViewData["ServiceId"] = new SelectList(_context.Services, "ServiceId", "ServiceId", serviceRequest.ServiceId);
            return View(serviceRequest);
        }

        // GET: AdminQL/ServiceRequests/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var serviceRequest = await _context.ServiceRequests
                .Include(s => s.Customer)
                .Include(s => s.ProcessedByNavigation)
                .Include(s => s.Schedule)
                .Include(s => s.Service)
                .FirstOrDefaultAsync(m => m.RequestId == id);
            if (serviceRequest == null)
            {
                return NotFound();
            }

            return View(serviceRequest);
        }

        // POST: AdminQL/ServiceRequests/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var serviceRequest = await _context.ServiceRequests.FindAsync(id);
            if (serviceRequest != null)
            {
                _context.ServiceRequests.Remove(serviceRequest);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ServiceRequestExists(int id)
        {
            return _context.ServiceRequests.Any(e => e.RequestId == id);
        }
    }
}
