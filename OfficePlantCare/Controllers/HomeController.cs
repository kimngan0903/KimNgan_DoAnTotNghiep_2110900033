using Microsoft.AspNetCore.Authorization;
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
        public async Task<IActionResult> Index()
        {
        var customerName = HttpContext.Session.GetString("CustomerName");

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

            // 1. Tính tổng số lần sử dụng từ OrderDetails (dựa trên Quantity)
            var orderDetailCounts = _context.OrderDetails
                .GroupBy(od => od.ServiceId)
                .Select(g => new
                {
                    ServiceId = g.Key,
                    Count = g.Sum(od => od.Quantity)
                })
                .ToList();

            // 2. Tính tổng số lần sử dụng từ Contracts (đếm số hợp đồng)
            var contractDetailCounts = await _context.ContractDetails
                 .GroupBy(c => c.ServiceId)
                 .Select(g => new
                 {
                     ServiceId = g.Key,
                     Count = g.Count()
                 })
                 .ToListAsync();

            // Lấy tất cả dịch vụ
            var service = _context.Services
                .Select(s => new
                {
                    s.ServiceId,
                    s.ServiceName
                })
                .ToList();

            // Kết hợp dữ liệu trong bộ nhớ
            var serviceUsage = service.Select(s => new
            {
                s.ServiceId,
                s.ServiceName,
                TotalUsage = orderDetailCounts
                    .Where(od => od.ServiceId == s.ServiceId)
                    .Select(od => od.Count)
                    .FirstOrDefault() +
                    contractDetailCounts
                    .Where(c => c.ServiceId == s.ServiceId)
                    .Select(c => c.Count)
                    .FirstOrDefault()
            })
            .Where(s => s.TotalUsage > 0) // Chỉ lấy dịch vụ có số lần sử dụng lớn hơn 0
            .OrderByDescending(s => s.TotalUsage) // Sắp xếp giảm dần theo số lần sử dụng
            .Take(4) // Lấy 4 dịch vụ phổ biến nhất
            .ToList();

            // Khởi tạo danh sách servicePercentages
            List<dynamic> servicePercentages;

            if (serviceUsage.Any())
            {
                // Tính tổng số lần sử dụng của 4 dịch vụ này
                var totalUsage = serviceUsage.Sum(s => s.TotalUsage);

                // Tính phần trăm cho từng dịch vụ
                servicePercentages = serviceUsage.Select(s => (dynamic)new
                {
                    s.ServiceId,
                    s.ServiceName,
                    Percentage = totalUsage > 0
                        ? (int)Math.Round((double)s.TotalUsage / totalUsage * 100)
                        : 0
                }).ToList();
            }
            else
            {
                // Nếu không có dữ liệu, để danh sách rỗng
                servicePercentages = new List<dynamic>();
            }
            ViewData["ServiceCategories"] = categories; // Truyền vào ViewData
            ViewData["Banners"] = banners; // Truyền danh sách banners vào ViewData
            ViewData["Services"] = services; // Truyền danh sách dịch vụ vào ViewData
            ViewData["News"] = latestNews;
            ViewData["ServicePercentages"] = servicePercentages;


            return View(); 
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

        //[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        //public IActionResult Error()
        //{
        //    return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        //}
    }
}

