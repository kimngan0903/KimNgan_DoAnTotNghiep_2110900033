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
    public class ServiceRequestsController : Controller
    {
        private readonly OfficePlantCareContext _context;

        public ServiceRequestsController(OfficePlantCareContext context)
        {
            _context = context;
        }

        // GET: ServiceRequests
        public async Task<IActionResult> Index()
        {
            var officePlantCareContext = _context.ServiceRequests.Include(s => s.Customer).Include(s => s.ProcessedByNavigation).Include(s => s.Schedule).Include(s => s.Service);
            return View(await officePlantCareContext.ToListAsync());
        }

        // GET: ServiceRequests/Details/5
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

        // GET: ServiceRequests/Create
        [HttpGet]
        public IActionResult Create()
        {

            ViewData["ServiceId"] = new SelectList(_context.Services, "ServiceId", "ServiceName");
            return View();
        }


        // POST: ServiceRequests/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        // POST: ServiceRequests/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ServiceId, Quantity, Notes")] ServiceRequest request)
        {
            if (request.Quantity <= 0)
            {
                ModelState.AddModelError("Quantity", "Số lượng phải lớn hơn 0.");
                return View(request);
            }

            if (!ModelState.IsValid)
            {
                return View(request);
            }

            var customerId = HttpContext.Session.GetInt32("CustomerId");
            if (customerId == null)
            {
                return RedirectToAction("Index", "Login");
            }

            var roleId = HttpContext.Session.GetInt32("RoleId");

            // Lấy giá dịch vụ từ bảng ServicePrice
            var servicePrice = await _context.ServicePrices
                .Where(sp => sp.ServiceId == request.ServiceId)
                .OrderByDescending(sp => sp.PriceId) // Lấy giá mới nhất nếu có nhiều dòng
                .FirstOrDefaultAsync();

            if (servicePrice == null || servicePrice.Price == null)
            {
                ModelState.AddModelError("", "Không tìm thấy giá dịch vụ hoặc dịch vụ chưa có giá.");
                return View(request);
            }

            decimal price = servicePrice.Price.Value; // Lấy giá từ ServicePrice

            var serviceRequest = new ServiceRequest
            {
                CustomerId = customerId,
                ServiceId = request.ServiceId,
                Quantity = request.Quantity,
                Notes = request.Notes,
                RequestDate = DateTime.Now,
                //Status = "Chờ xử lý",
                //Price = price, // Gán giá từ ServicePrice
                //TotalAmount = request.Quantity * price, // Tính tổng tiền
                UpdatedAt = DateTime.Now
            };

            _context.ServiceRequests.Add(serviceRequest);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Success));
        }



        public IActionResult Success()
        {
            return View();
        }

        // GET: ServiceRequests/Edit/5
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

        // POST: ServiceRequests/Edit/5
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

        // GET: ServiceRequests/Delete/5
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

        // POST: ServiceRequests/Delete/5
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
