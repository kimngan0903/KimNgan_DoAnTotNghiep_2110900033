using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OfficePlantCare.Models;

namespace OfficePlantCare.Controllers
{
    public class AboutUsController : Controller
    {
        private readonly OfficePlantCareContext _context;

        public AboutUsController(OfficePlantCareContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            // Lấy danh sách danh mục dịch vụ
            var categories = await _context.ServiceCategories
                .Include(c => c.Services)
                .ToListAsync();

            // 1. Tính tổng số lần sử dụng từ OrderDetails (dựa trên Quantity)
            var orderDetailCounts = await _context.OrderDetails
                .GroupBy(od => od.ServiceId)
                .Select(g => new
                {
                    ServiceId = g.Key,
                    Count = g.Sum(od => od.Quantity)
                })
                .ToListAsync();

            // 2. Tính tổng số lần sử dụng từ ContractDetails (đếm số bản ghi)
            var contractDetailCounts = await _context.ContractDetails
                .GroupBy(c => c.ServiceId)
                .Select(g => new
                {
                    ServiceId = g.Key,
                    Count = g.Count()
                })
                .ToListAsync();

            // Lấy tất cả dịch vụ
            var services = await _context.Services
                .Select(s => new
                {
                    s.ServiceId,
                    s.ServiceName
                })
                .ToListAsync();

            // Kết hợp dữ liệu trong bộ nhớ
            var serviceUsage = services.Select(s => new
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

            ViewData["ServiceCategories"] = categories;
            ViewData["ServicePercentages"] = servicePercentages;

            return View();
        }
    }
}