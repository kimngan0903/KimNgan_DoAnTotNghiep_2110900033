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
    public class ContractsController : BaseController
    {
        private readonly OfficePlantCareContext _context;
        private readonly List<int> validContractServiceIds = new List<int> { 1, 3, 10, 12, 14 };

        public ContractsController(OfficePlantCareContext context)
        {
            _context = context;
        }

        // GET: AdminQL/Contracts
        //public async Task<IActionResult> Index()
        //{
        //    var officePlantCareContext = _context.Contracts.Include(c => c.Customer).Include(c => c.PaymentMethod).Include(c => c.Service);
        //    return View(await officePlantCareContext.ToListAsync());
        //}
        // GET: AdminQL/Contracts
        public async Task<IActionResult> Index(string name, int page = 1)
        {
            // Số ghi trên 1 trang
            int limit = 5;

            // Tạo query cơ bản
            IQueryable<Contract> query = _context.Contracts.OrderBy(c => c.ContractCode);

            // Nếu có tham số name trên URL, thêm điều kiện lọc
            if (!string.IsNullOrEmpty(name))
            {
                query = query.Where(c => c.ContractCode.Contains(name));
            }

            // Chuyển query sang danh sách
            var contract = await query.ToListAsync(); // Dùng ToListAsync() của EF Core

            // Sử dụng ToPagedList để phân trang (không bất đồng bộ)
            var pagedContract = contract.ToPagedList(page, limit);

            // Gửi từ khóa tìm kiếm cho View qua ViewBag
            ViewBag.keyword = name;

            return View(pagedContract);
        }

        // GET: AdminQL/Contracts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var contract = await _context.Contracts
                .Include(c => c.Customer)
                .Include(c => c.PaymentMethod)
                .Include(c => c.Service)
                .FirstOrDefaultAsync(m => m.ContractId == id);
            if (contract == null)
            {
                return NotFound();
            }
            return PartialView("_Details", contract);
        }

        // GET: AdminQL/Contracts/Create
        public IActionResult Create()
        {
            var contractServices = _context.Services
                .Where(s => validContractServiceIds.Contains(s.ServiceId)) // Lọc dịch vụ hợp đồng
                .ToList();

            ViewData["CustomerId"] = new SelectList(_context.Customers.Where(c => c.RoleId == 4), "CustomerId", "CustomerName"); // Chỉ khách ký hợp đồng
            ViewData["PaymentMethodId"] = new SelectList(_context.PaymentMethods, "PaymentMethodId", "MethodName");
            ViewData["ServiceId"] = new SelectList(contractServices, "ServiceId", "ServiceName");

            return View();
        }


        // POST: AdminQL/Contracts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ContractId,CustomerId,ServiceId,FixedPrice,DurationUnit,Duration,StartDate,PaymentMethodId,PaidAmount,Status")] Contract contract)
        {
            if (!validContractServiceIds.Contains(contract.ServiceId))
            {
                ModelState.AddModelError("ServiceId", "Dịch vụ không hợp lệ cho hợp đồng.");
            }

            if (ModelState.IsValid)
            {
                contract.TotalAmount = contract.FixedPrice * contract.Duration;
                contract.RemainingAmount = contract.TotalAmount - (contract.PaidAmount ?? 0);
                contract.CreatedAt = DateTime.Now;
                contract.UpdatedAt = DateTime.Now;

                _context.Add(contract);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["CustomerId"] = new SelectList(_context.Customers.Where(c => c.RoleId == 4), "CustomerId", "CustomerName", contract.CustomerId);
            ViewData["PaymentMethodId"] = new SelectList(_context.PaymentMethods, "PaymentMethodId", "MethodName", contract.PaymentMethodId);
            ViewData["ServiceId"] = new SelectList(_context.Services.Where(s => validContractServiceIds.Contains(s.ServiceId)), "ServiceId", "ServiceName", contract.ServiceId);

            return View(contract);
        }


        // GET: AdminQL/Contracts/Edit/5
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
            ViewData["CustomerId"] = new SelectList(_context.Customers, "CustomerId", "CustomerName", contract.CustomerId);
            ViewData["PaymentMethodId"] = new SelectList(_context.PaymentMethods, "PaymentMethodId", "MethodName", contract.PaymentMethodId);
            ViewData["ServiceId"] = new SelectList(_context.Services, "ServiceId", "ServiceName", contract.ServiceId);
            return PartialView("_Edit", contract);
        }

        // POST: AdminQL/Contracts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ContractId,ContractCode,CustomerId,ServiceId,FixedPrice,DurationUnit,Duration,StartDate,EndDate,PaymentMethodId,TotalAmount,PaidAmount,RemainingAmount,Status,CreatedAt,UpdatedAt")] Contract contract)
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
            ViewData["CustomerId"] = new SelectList(_context.Customers, "CustomerId", "CustomerName", contract.CustomerId);
            ViewData["PaymentMethodId"] = new SelectList(_context.PaymentMethods, "PaymentMethodId", "MethodName", contract.PaymentMethodId);
            ViewData["ServiceId"] = new SelectList(_context.Services, "ServiceId", "ServiceName", contract.ServiceId);
            return View(contract);
        }

        // GET: AdminQL/Contracts/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var contract = await _context.Contracts
                .Include(c => c.Customer)
                .Include(c => c.PaymentMethod)
                .Include(c => c.Service)
                .FirstOrDefaultAsync(m => m.ContractId == id);
            if (contract == null)
            {
                return NotFound();
            }

            return PartialView("_Delete", contract);
        }

        // POST: AdminQL/Contracts/Delete/5
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
