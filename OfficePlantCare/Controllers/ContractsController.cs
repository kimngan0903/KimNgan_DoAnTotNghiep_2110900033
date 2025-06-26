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
    public class ContractsController : Controller
    {
        private readonly OfficePlantCareContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ContractsController(OfficePlantCareContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        // GET: Contracts
        public async Task<IActionResult> Index()
        {
            int? customerId = HttpContext.Session.GetInt32("CustomerId");
            if (customerId == null)
            {
                return RedirectToAction("Index", "Login"); // Nếu chưa đăng nhập, chuyển hướng về trang đăng nhập
            }

            var contracts = await _context.Contracts
                .Where(c => c.CustomerId == customerId.Value) // Chỉ lấy đơn hàng của khách đang đăng nhập
                .Include(c => c.ContractDetails)
                .Include(c => c.Customer)
                .Include(c => c.PaymentMethod)
                .ToListAsync();
            var categories = _context.ServiceCategories
                                         .Include(c => c.Services) // Nạp luôn danh sách dịch vụ thuộc danh mục đó
                                         .ToList();

            ViewData["ServiceCategories"] = categories; // Truyền vào ViewData
            return View(contracts);
        }

        // GET: Contracts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var categories = _context.ServiceCategories
                                        .Include(c => c.Services) // Nạp luôn danh sách dịch vụ thuộc danh mục đó
                                        .ToList();

            ViewData["ServiceCategories"] = categories; // Truyền vào ViewData
            var customerId = _httpContextAccessor.HttpContext.Session.GetInt32("CustomerId");
            if (!customerId.HasValue)
            {
                return RedirectToAction("Index", "Login");
            }

            var contract = await _context.Contracts
                     .Include(c => c.ContractDetails)
                         .ThenInclude(od => od.Service)
                     .Include(c => c.ContractDetails)
                         .ThenInclude(od => od.Price)
                     .Include(c => c.Customer)
                     .Include(c => c.PaymentMethod)
                     .FirstOrDefaultAsync(c => c.ContractId == id && c.CustomerId == customerId.Value);
            if (contract == null)
            {
                return NotFound();
            }

            return View(contract);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CancelContract(int contractId)
        {
            var customerId = _httpContextAccessor.HttpContext.Session.GetInt32("CustomerId");
            if (!customerId.HasValue)
            {
                return RedirectToAction("Index", "Login");
            }

            var contract = await _context.Contracts
                .Include(c => c.CareSchedules)
                .FirstOrDefaultAsync(c => c.ContractId == contractId && c.CustomerId == customerId.Value);

            if (contract == null)
            {
                return NotFound();
            }

            if (contract.Status != "Chờ xử lý" || contract.PaymentStatus == "Đã thanh toán")
            {
                TempData["ErrorMessage"] = "Không thể hủy hợp đồng này.";
                return RedirectToAction("Index");
            }

            contract.Status = "Hủy";

            if (contract.CareSchedules.Any())
            {
                _context.CareSchedules.RemoveRange(contract.CareSchedules);
            }

            _context.Update(contract);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Hợp đồng và các lịch chăm sóc đã được hủy thành công.";
            return RedirectToAction("Index");
        }

        // GET: Contracts/Create
        public IActionResult Create()
        {
            ViewData["CustomerId"] = new SelectList(_context.Customers, "CustomerId", "CustomerName");
            ViewData["PaymentMethodId"] = new SelectList(_context.PaymentMethods, "PaymentMethodId", "PaymentMethodId");
            return View();
        }

        // POST: Contracts/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ContractId,ContractCode,CustomerId,CreatedDate,TotalAmount,DurationUnit,Duration,StartDate,EndDate,PaymentMethodId,PaymentStatus,Status")] Contract contract)
        {
            if (ModelState.IsValid)
            {
                _context.Add(contract);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CustomerId"] = new SelectList(_context.Customers, "CustomerId", "CustomerId", contract.CustomerId);
            ViewData["PaymentMethodId"] = new SelectList(_context.PaymentMethods, "PaymentMethodId", "PaymentMethodId", contract.PaymentMethodId);
            return View(contract);
        }

        // GET: Contracts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var contract = await _context.Contracts.FindAsync(id);
            if (contract == null)
            {
                return NotFound();
            }
            ViewData["CustomerId"] = new SelectList(_context.Customers, "CustomerId", "CustomerId", contract.CustomerId);
            ViewData["PaymentMethodId"] = new SelectList(_context.PaymentMethods, "PaymentMethodId", "PaymentMethodId", contract.PaymentMethodId);
            return View(contract);
        }

        // POST: Contracts/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ContractId,ContractCode,CustomerId,CreatedDate,TotalAmount,DurationUnit,Duration,StartDate,EndDate,PaymentMethodId,PaymentStatus,Status")] Contract contract)
        {
            if (id != contract.ContractId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(contract);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ContractExists(contract.ContractId))
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
            ViewData["CustomerId"] = new SelectList(_context.Customers, "CustomerId", "CustomerId", contract.CustomerId);
            ViewData["PaymentMethodId"] = new SelectList(_context.PaymentMethods, "PaymentMethodId", "PaymentMethodId", contract.PaymentMethodId);
            return View(contract);
        }

        // GET: Contracts/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var contract = await _context.Contracts
                .Include(c => c.Customer)
                .Include(c => c.PaymentMethod)
                .FirstOrDefaultAsync(m => m.ContractId == id);
            if (contract == null)
            {
                return NotFound();
            }

            return View(contract);
        }

        // POST: Contracts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var contract = await _context.Contracts.FindAsync(id);
            if (contract != null)
            {
                _context.Contracts.Remove(contract);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ContractExists(int id)
        {
            return _context.Contracts.Any(e => e.ContractId == id);
        }
    }
}