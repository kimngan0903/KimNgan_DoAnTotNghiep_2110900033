using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using OfficePlantCare.Models;

namespace OfficePlantCare.Controllers
{
    public class ServicesController : Controller
    {
        private readonly OfficePlantCareContext _context;

        public ServicesController(OfficePlantCareContext context)
        {
            _context = context;
        }

        // GET: AdminQL/Services
        public async Task<IActionResult> Index()
        {
            var categories = _context.ServiceCategories
                                         .Include(c => c.Services) // N?p luôn danh sách d?ch v? thu?c danh m?c ?ó
                                         .ToList();
            ViewData["ServiceCategories"] = categories; // Truy?n vào ViewData

            var officePlantCareContext = _context.Services.Include(s => s.CategoryService);
            return View(await officePlantCareContext.ToListAsync());
        }
        // GET: Services/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // Truy vấn service với ServiceCategory và ServiceDescriptions
            var services = await _context.Services
                .Include(s => s.CategoryService)  // Bao gồm ServiceCategory
                .Include(s => s.ServiceDescriptions)  // Bao gồm ServiceDescriptions
                .Include(s => s.ServicePrices)
                .FirstOrDefaultAsync(s => s.ServiceId == id);

            if (services == null || services.ServiceDescriptions == null)
            {
                return NotFound();
            }

            // Kiểm tra xem Content có null hoặc rỗng không trước khi thay thế
            foreach (var description in services.ServiceDescriptions)
            {
                if (!string.IsNullOrEmpty(description.Content))
                {
                    description.Content = description.Content.Replace(Environment.NewLine, "<br>");
                }
            }

            return View(services);
        }

        [HttpGet]
        public IActionResult GetPrice(string serviceType, string treeSize, string officeSize, int numberOfTrees)
        {
            var pricePerUnit = _context.ServicePrices
                .Where(p => p.ServiceType == serviceType && p.TreeSize == treeSize && p.OfficeSize == officeSize)
                .Select(p => p.Price)
                .FirstOrDefault();

            if (pricePerUnit != null)
            {
                var totalPrice = pricePerUnit * numberOfTrees; // Nhân giá với số lượng cây
                return Json(new { success = true, price = totalPrice });
            }

            return Json(new { success = false, message = "Không tìm thấy giá" });
        }



        [HttpGet]
        public IActionResult GetDiscountedPrice(int serviceId, string treeSize, string officeSize, int numberOfTrees)
        {
            if (numberOfTrees <= 0) return BadRequest("Số lượng cây phải lớn hơn 0.");

            var basePriceEntry = _context.ServicePrices
                .Where(p => p.ServiceId == serviceId
                         && p.ServiceType == "Lẻ"
                         && p.TreeSize == treeSize
                         && p.OfficeSize == officeSize
                         && p.NumberOfTrees == null) // Chỉ lấy giá cho 1 cây
                .FirstOrDefault();

            if (basePriceEntry == null) return NotFound("Không tìm thấy giá cho dịch vụ này.");

            decimal basePrice = basePriceEntry.Price??0;
            decimal discountPercent = 0;
            // Chỉ áp dụng giảm giá khi số lượng cây đủ lớn
            if (numberOfTrees > 200)
                discountPercent = 0.07m; // 7% giảm giá
            else if (numberOfTrees > 100)
                discountPercent = 0.05m; // 5% giảm giá
            else if (numberOfTrees > 50)
                discountPercent = 0.03m; // 3% giảm giá

            decimal finalPrice = basePrice * numberOfTrees * (1 - discountPercent);

            return Json(finalPrice);
        }

        // GET: AdminQL/Services/Create
        public IActionResult Create()
        {
            ViewData["CategoryServiceId"] = new SelectList(_context.ServiceCategories, "CategoryServiceId", "CategoryServiceName");
            return View();
        }

        // POST: AdminQL/Services/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ServiceId,ServiceName,Description,CreatedDate,UpdatedDate,Status,CategoryServiceId,Image")] Service service)
        {
            if (ModelState.IsValid)
            {
                _context.Add(service);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoryServiceId"] = new SelectList(_context.ServiceCategories, "CategoryServiceId", "CategoryServiceName", service.CategoryServiceId);
            return View(service);
        }
    }
}
