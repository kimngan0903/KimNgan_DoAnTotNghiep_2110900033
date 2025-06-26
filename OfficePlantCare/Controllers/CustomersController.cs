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
    public class CustomersController : Controller
    {
        private readonly OfficePlantCareContext _context;

        public CustomersController(OfficePlantCareContext context)
        {
            _context = context;
        }

        // GET: Customers
        public async Task<IActionResult> Index(int id)
        {
            int? customerId = HttpContext.Session.GetInt32("CustomerId");
            if (customerId == null)
            {
                return RedirectToAction("Index", "Login"); 
            }

            var careSchedules = _context.CareSchedules
                .Include(c => c.Contract)
                .Include(c => c.Order)
                .Include(c => c.Staff)
                .Where(c =>
                    (c.Contract != null && c.Contract.CustomerId == id) ||
                    (c.Order != null && c.Order.CustomerId == id) 
                );
            var categories = _context.ServiceCategories
                           .Include(c => c.Services)
                           .ToList();

            ViewData["ServiceCategories"] = categories;
            return View(await careSchedules.ToListAsync());
        }

        // GET: Customers/Details/5
        public async Task<IActionResult> Details(int? customerId)
        {
            if (customerId == null)
            {
                return NotFound();
            }

            var customer = await _context.Customers
                .FirstOrDefaultAsync(m => m.CustomerId == customerId);
            if (customer == null)
            {
                return NotFound();
            }
            var categories = _context.ServiceCategories
                                   .Include(c => c.Services)
                                   .ToList();

            ViewData["ServiceCategories"] = categories;
            return View(customer);
        }

        // GET: Customers/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Customers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CustomerId,CustomerName,Email,Phone,Address,PasswordHash,CreatedDate,Status")] Customer customer)
        {
            if (ModelState.IsValid)
            {
                // Mã hóa mật khẩu trước khi lưu
                customer.PasswordHash = GetSHA256Hash(customer.PasswordHash);
                customer.CreatedDate = DateTime.Now;
                _context.Add(customer);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(customer);
        }

        // GET: Customers/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var customer = await _context.Customers.FindAsync(id);
            if (customer == null)
            {
                return NotFound();
            }
            return View(customer);
        }

        // POST: Customers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("CustomerId,CustomerName,Email,Phone,Address,PasswordHash,CreatedDate,Status")] Customer customer)
        {
            if (id != customer.CustomerId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var existingCustomer = await _context.Customers.FindAsync(id);
                    if (existingCustomer == null)
                    {
                        return NotFound();
                    }

                    existingCustomer.CustomerName = customer.CustomerName ?? existingCustomer.CustomerName;
                    existingCustomer.Email = customer.Email ?? existingCustomer.Email;
                    existingCustomer.Phone = customer.Phone ?? existingCustomer.Phone;
                    existingCustomer.Address = customer.Address ?? existingCustomer.Address;
                    existingCustomer.CreatedDate = customer.CreatedDate ?? existingCustomer.CreatedDate;
                    existingCustomer.PasswordHash = customer.PasswordHash ?? existingCustomer.PasswordHash;
                    existingCustomer.Status = customer.Status ?? existingCustomer.Status;

                    await _context.SaveChangesAsync();

                    return RedirectToAction(nameof(Details), new { customerId = customer.CustomerId });
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Đã xảy ra lỗi khi lưu dữ liệu: " + ex.Message);
                }
            }
            return View("Details", customer);
        }

        // GET: Customers/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var customer = await _context.Customers
                .FirstOrDefaultAsync(m => m.CustomerId == id);
            if (customer == null)
            {
                return NotFound();
            }

            return View(customer);
        }

        // POST: Customers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var customer = await _context.Customers.FindAsync(id);
            if (customer != null)
            {
                _context.Customers.Remove(customer);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CustomerExists(int id)
        {
            return _context.Customers.Any(e => e.CustomerId == id);
        }
        private string GetSHA256Hash(string input)
        {
            using (var sha256 = System.Security.Cryptography.SHA256.Create())
            {
                var hashBytes = sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(input));
                return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
            }
        }

    }
}
