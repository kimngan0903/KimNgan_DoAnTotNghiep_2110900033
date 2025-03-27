using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OfficePlantCare.Models;
using X.PagedList;
using X.PagedList.Extensions;

namespace OfficePlantCare.Controllers
{
    public class ServicesController : Controller
    {
        private readonly OfficePlantCareContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ServicesController(OfficePlantCareContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

              // GET: Services
        public async Task<IActionResult> Index(string sortOrder, int? categoryId, string searchString, int page = 1)
        {
            int pageSize = 6; // Số dịch vụ trên mỗi trang (giống NewsController)

            ViewData["CurrentSort"] = sortOrder;
            ViewData["NameSortParm"] = string.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewData["CurrentFilter"] = searchString;

            var services = _context.Services
                .Include(s => s.CategoryService)
                .Include(s => s.ServicePrices)
                .Include(s => s.Feedbacks)
                .AsQueryable();

            // Lọc theo danh mục nếu categoryId được cung cấp
            if (categoryId.HasValue)
            {
                services = services.Where(s => s.CategoryServiceId == categoryId.Value);
                ViewBag.SelectedCategoryId = categoryId.Value;
            }

            // Tìm kiếm theo từ khóa
            if (!string.IsNullOrEmpty(searchString))
            {
                services = services.Where(s => s.ServiceName.Contains(searchString));
            }

            // Sắp xếp
            switch (sortOrder)
            {
                case "name_desc":
                    services = services.OrderByDescending(s => s.ServiceName);
                    break;
                default:
                    services = services.OrderBy(s => s.ServiceName);
                    break;
            }

            // Lấy danh sách danh mục để hiển thị trong sidebar
            var categories = await _context.ServiceCategories
                .Include(c => c.Services)
                .ToListAsync();
            ViewData["ServiceCategories"] = categories;

            // Tính tổng số dịch vụ
            ViewBag.TotalServices = await services.CountAsync();

                       // Chuyển query sang danh sách
            var servicesList = await services.ToListAsync(); // Dùng ToListAsync() giống NewsController

            // Phân trang (không bất đồng bộ)
            var pagedServices = servicesList.ToPagedList(page, pageSize);

            return View(pagedServices);
        }

        // GET: Services/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var service = await _context.Services
                .Include(s => s.CategoryService)
                .Include(s => s.ServiceDescriptions)
                .Include(s => s.ServicePrices)
                .Include(s => s.Feedbacks)
                    .ThenInclude(f => f.Customer)
                .FirstOrDefaultAsync(s => s.ServiceId == id);

            if (service == null || service.ServiceDescriptions == null)
            {
                return NotFound();
            }

            var categories = _context.ServiceCategories
                .Include(c => c.Services)
                .ToList();
            ViewData["ServiceCategories"] = categories;

            ViewBag.CustomerLogin = _httpContextAccessor.HttpContext.Session.GetInt32("CustomerLogin");
            ViewBag.RoleId = _httpContextAccessor.HttpContext.Session.GetInt32("RoleId");
            ViewBag.CustomerId = _httpContextAccessor.HttpContext.Session.GetInt32("CustomerId");
            ViewBag.ServiceType = _httpContextAccessor.HttpContext.Session.GetString("ServiceType");
            ViewBag.TreeSize = _httpContextAccessor.HttpContext.Session.GetString("TreeSize");
            ViewBag.NumberOfTrees = _httpContextAccessor.HttpContext.Session.GetInt32("NumberOfTrees");
            ViewBag.OfficeSize = _httpContextAccessor.HttpContext.Session.GetString("OfficeSize");

            foreach (var description in service.ServiceDescriptions)
            {
                if (!string.IsNullOrEmpty(description.Content))
                {
                    description.Content = description.Content.Replace("\r\n", "<br>").Replace("\n", "<br>");
                }
            }

            return View(service);
        }

        // POST: Services/AddFeedback
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddFeedback(int serviceId, int? customerId, int rating, string content)
        {
            var sessionCustomerId = _httpContextAccessor.HttpContext.Session.GetInt32("CustomerId");
            var roleId = _httpContextAccessor.HttpContext.Session.GetInt32("RoleId");

            int? finalCustomerId = null;

            if (!sessionCustomerId.HasValue || customerId == 0)
            {
                finalCustomerId = null;
            }
            else if (roleId.HasValue)
            {
                if (roleId.Value == 3)
                {
                    finalCustomerId = null;
                }
                else
                {
                    finalCustomerId = sessionCustomerId.Value;
                }
            }

            var feedback = new Feedback
            {
                ServiceId = serviceId,
                CustomerId = finalCustomerId,
                Rating = rating,
                Content = content,
                FeedbackDate = DateTime.Now,
                Status = finalCustomerId == null ? null : "Chờ duyệt"
            };

            _context.Feedbacks.Add(feedback);
            await _context.SaveChangesAsync();

            return RedirectToAction("Details", new { id = serviceId });
        }
    }
}