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

            ViewData["ServiceCategories"] = categories; // Truyền vào ViewData
            ViewData["Banners"] = banners; // Truyền danh sách banners vào ViewData
            ViewData["Services"] = services; // Truyền danh sách dịch vụ vào ViewData

            // Nếu là khách hợp đồng hoặc khách đặt dịch vụ lẻ, lấy dữ liệu từ DashboardService
            if (roleId == 4 || roleId == 5)
            {
                var data = _dashboardService.GetDashboardData();
                ViewData["Index"] = data;
            }

            return View(); // Hiển thị chung 1 view
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

