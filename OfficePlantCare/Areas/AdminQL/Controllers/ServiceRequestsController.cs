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
    public class ServiceRequestsController : BaseController
    {
        private readonly OfficePlantCareContext _context;
        private readonly CareScheduleController _careSchedulesController;

        public ServiceRequestsController(OfficePlantCareContext context, CareScheduleController careSchedulesController)
        {
            _context = context;
            _careSchedulesController = careSchedulesController;
        }

        // GET: AdminQL/ServiceRequests
        public async Task<IActionResult> Index(string name, int page = 1, int? confirmPayment = null)
        {
            int limit = 5;

            IQueryable<ServiceRequest> query = _context.ServiceRequests
                .Include(s => s.Customer)
                .Include(s => s.Location)
                .Include(s => s.PaymentMethod)
                .Include(s => s.Price)
                .Include(s => s.Service)
                .OrderBy(c => c.Customer.CustomerName);

            if (!string.IsNullOrEmpty(name))
            {
                query = query.Where(c => c.Customer.CustomerName.Contains(name));
            }

            // Xử lý xác nhận thanh toán
            if (confirmPayment.HasValue)
            {
                var serviceRequests = await _context.ServiceRequests
                    .Where(s => s.CustomerId == confirmPayment.Value)
                    .ToListAsync();

                if (serviceRequests.Any())
                {
                    foreach (var serviceRequest in serviceRequests)
                    {
                        if (serviceRequest.PaymentStatus == "Chưa thanh toán")
                        {
                            serviceRequest.PaymentStatus = "Đã thanh toán";
                            serviceRequest.UpdatedAt = DateTime.Now;
                            _context.Update(serviceRequest);
                        }
                    }
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Xác nhận thanh toán thành công cho đơn hàng của khách hàng!";
                }
                else
                {
                    TempData["ErrorMessage"] = "Không tìm thấy yêu cầu nào để cập nhật cho khách hàng này.";
                }
            }
            var requests = await query.ToListAsync();
            var pagedRequests = requests.ToPagedList(page, limit);

            ViewBag.keyword = name;
            return View(pagedRequests);
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
                .Include(s => s.Location)
                .Include(s => s.PaymentMethod)
                .Include(s => s.Price)
                .Include(s => s.Service)
                .FirstOrDefaultAsync(m => m.RequestId == id);
            if (serviceRequest == null)
            {
                return NotFound();
            }

            return PartialView("_Details", serviceRequest);
        }

        // GET: AdminQL/ServiceRequests/Create
        public IActionResult Create()
        {
            ViewData["CustomerId"] = new SelectList(_context.Customers, "CustomerId", "CustomerName");
            ViewData["LocationId"] = new SelectList(_context.Locations, "LocationId", "LocationName");
            ViewData["PaymentMethodId"] = new SelectList(_context.PaymentMethods, "PaymentMethodId", "MethodName");
            ViewData["PriceId"] = new SelectList(_context.ServicePrices, "PriceId", "PriceId");
            ViewData["ServiceId"] = new SelectList(_context.Services, "ServiceId", "ServiceName");
            return View();
        }

        // POST: AdminQL/ServiceRequests/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("RequestId,CustomerId,ServiceId,LocationId,PriceId,Quantity,TotalAmount,Notes,RequestDate,UpdatedAt,Status,PaymentMethodId,PaymentStatus")] ServiceRequest serviceRequest)
        {
            if (ModelState.IsValid)
            {
                _context.Add(serviceRequest);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Thêm yêu cầu phát sinh thành công!";
                return RedirectToAction(nameof(Index));
            }

            ViewData["CustomerId"] = new SelectList(_context.Customers, "CustomerId", "CustomerName", serviceRequest.CustomerId);
            ViewData["LocationId"] = new SelectList(_context.Locations, "LocationId", "LocationName", serviceRequest.LocationId);
            ViewData["PaymentMethodId"] = new SelectList(_context.PaymentMethods, "PaymentMethodId", "MethodName", serviceRequest.PaymentMethodId);
            ViewData["PriceId"] = new SelectList(_context.ServicePrices, "PriceId", "PriceId", serviceRequest.PriceId);
            ViewData["ServiceId"] = new SelectList(_context.Services, "ServiceId", "ServiceName", serviceRequest.ServiceId);
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
            ViewData["CustomerId"] = new SelectList(_context.Customers, "CustomerId", "CustomerName", serviceRequest.CustomerId);
            ViewData["LocationId"] = new SelectList(_context.Locations, "LocationId", "LocationName", serviceRequest.LocationId);
            ViewData["PaymentMethodId"] = new SelectList(_context.PaymentMethods, "PaymentMethodId", "MethodName", serviceRequest.PaymentMethodId);
            ViewData["PriceId"] = new SelectList(_context.ServicePrices, "PriceId", "PriceId", serviceRequest.PriceId);
            ViewData["ServiceId"] = new SelectList(_context.Services, "ServiceId", "ServiceName", serviceRequest.ServiceId);
            return PartialView("_Edit", serviceRequest);
        }

        // POST: AdminQL/ServiceRequests/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("RequestId,CustomerId,ServiceId,LocationId,PriceId,Quantity,TotalAmount,Notes,RequestDate,UpdatedAt,Status,PaymentMethodId,PaymentStatus")] ServiceRequest serviceRequest)
        {
            if (id != serviceRequest.RequestId) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(serviceRequest);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Cập nhật yêu cầu phát sinh thành công!";
                    // Gọi phương thức để cập nhật trạng thái CareSchedule
                    bool updated = await _careSchedulesController.UpdateCareScheduleStatusForServiceRequest(serviceRequest.RequestId);
                    if (updated)
                    {
                        TempData["SuccessMessage"] = "Trạng thái lịch chăm sóc đã được cập nhật.";
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ServiceRequestExists(serviceRequest.RequestId)) return NotFound();
                    throw;
                }
                return RedirectToAction(nameof(Index));
            }

            ViewData["CustomerId"] = new SelectList(_context.Customers, "CustomerId", "CustomerName", serviceRequest.CustomerId);
            ViewData["LocationId"] = new SelectList(_context.Locations, "LocationId", "LocationName", serviceRequest.LocationId);
            ViewData["PaymentMethodId"] = new SelectList(_context.PaymentMethods, "PaymentMethodId", "MethodName", serviceRequest.PaymentMethodId);
            ViewData["PriceId"] = new SelectList(_context.ServicePrices, "PriceId", "PriceId", serviceRequest.PriceId);
            ViewData["ServiceId"] = new SelectList(_context.Services, "ServiceId", "ServiceName", serviceRequest.ServiceId);
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
                .Include(s => s.Location)
                .Include(s => s.PaymentMethod)
                .Include(s => s.Price)
                .Include(s => s.Service)
                .FirstOrDefaultAsync(m => m.RequestId == id);
            if (serviceRequest == null)
            {
                return NotFound();
            }

            return PartialView("_Delete", serviceRequest);
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
            TempData["SuccessMessage"] = "Xóa yêu cầu phát sinh thành công!";

            return RedirectToAction(nameof(Index));
        }

        // GET: AdminQL/ServiceRequests/PrintInvoice/5
        public async Task<IActionResult> PrintInvoice(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var serviceRequests = await _context.ServiceRequests
                  .Include(s => s.Customer)
                  .Include(s => s.Location)
                  .Include(s => s.PaymentMethod)
                  .Include(s => s.Price)
                  .Include(s => s.Service)
                  .ThenInclude(s => s.ServiceDescriptions)
                  .FirstOrDefaultAsync(o => o.RequestId == id);
            if (serviceRequests == null)
            {
                return NotFound("Không tìm thấy yêu cầu dịch vụ nào chưa thanh toán cho khách hàng này.");
            }
            return View(serviceRequests);
        }        
        private bool ServiceRequestExists(int id)
        {
            return _context.ServiceRequests.Any(e => e.RequestId == id);
        }
    }
}