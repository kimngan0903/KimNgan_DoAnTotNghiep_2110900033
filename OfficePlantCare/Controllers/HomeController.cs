using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OfficePlantCare.Areas.AdminQL.Models;
using OfficePlantCare.Models;
using System.Diagnostics;

namespace OfficePlantCare.Controllers
{
    public class HomeController : Controller
    {
        private readonly OfficePlantCareContext _context;
        private readonly ILogger<HomeController> _logger;
        private readonly DashboardService _dashboardService;

        public HomeController(OfficePlantCareContext context, ILogger<HomeController> logger, DashboardService dashboardService)
        {
            _context = context;
            _logger = logger;
            _dashboardService = dashboardService;
        }
        // Trang chủ chung cho mọi role
        public async Task<IActionResult> Index()
        {
        var roleId = HttpContext.Session.GetInt32("RoleId");
            var contractCode = HttpContext.Session.GetString("ContractCode");

            var categories = await _context.ServiceCategories
                  .Include(c => c.Services) // Nạp luôn danh sách dịch vụ thuộc danh mục đó
                  .ToListAsync();
            var banners = await _context.Banners.ToListAsync(); // Lấy danh sách banner từ database

            var services = await _context.Services
                .Where(s => s.Status == "Đang hoạt động") // Lọc dịch vụ đang hoạt động
                .OrderByDescending(s => s.CreatedDate) // Sắp xếp theo ngày tạo mới nhất
                .Take(4) // Lấy 4 dịch vụ
                .ToListAsync();
            var latestNews = _context.News
               .OrderByDescending(n => n.CreatedDate)
               .Take(3)
               .ToList();

            ViewData["ServiceCategories"] = categories; // Truyền vào ViewData
            ViewData["Banners"] = banners; // Truyền danh sách banners vào ViewData
            ViewData["Services"] = services; // Truyền danh sách dịch vụ vào ViewData
            ViewData["News"] = latestNews;

            // Nếu là khách hợp đồng hoặc khách đặt dịch vụ lẻ, lấy dữ liệu từ DashboardService
            if (roleId == 4 || roleId == 5)
            {
                var data = _dashboardService.GetDashboardData();
                ViewData["Index"] = data;
            }

            return View(); // Hiển thị chung 1 view
        }
        // POST: Contact/SubmitContact
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SubmitContact(string Name, string Email, string Phone, string Address, string Message)
        {
            if (ModelState.IsValid)
            {
                // Tạo một bản ghi Contact mới
                var contact = new Contact
                {
                    Email = Email,
                    Phone = Phone,
                    Address = Address,
                    Description = $"Tên: {Name} - Nội dung: {Message}",
                    CreatedDate = DateTime.Now,
                    Status = "Chưa xử lý"
                };

                // Thêm vào cơ sở dữ liệu
                _context.Contacts.Add(contact);
                await _context.SaveChangesAsync();

                // Thêm thông báo thành công vào TempData
                TempData["SuccessMessage"] = "Yêu cầu liên hệ của bạn đã được gửi thành công! Chúng tôi sẽ liên hệ với bạn sớm nhất.";

                // Chuyển hướng về trang Index
                return RedirectToAction(nameof(Index));
            }

            // Nếu có lỗi, trả lại trang Index với dữ liệu hiện tại
            var categories = _context.ServiceCategories
                                     .Include(c => c.Services)
                                     .ToList();
            ViewData["ServiceCategories"] = categories;
            return View("Index");
        }
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}

