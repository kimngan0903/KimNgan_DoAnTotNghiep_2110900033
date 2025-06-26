using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OfficePlantCare.Models;
using X.PagedList.Extensions;

namespace OfficePlantCare.Controllers
{
    public class NewsController : Controller
    {
        private readonly OfficePlantCareContext _context;

        public NewsController(OfficePlantCareContext context)
        {
            _context = context;
        }

        // GET: News
        public async Task<IActionResult> Index(string search, int page = 1)
        {
            int pageSize = 6; // Số tin tức trên mỗi trang

            // Lấy danh sách tin tức
            IQueryable<News> newsQuery = _context.News
                                         .Where(n => n.Status == "Hiển thị")
                                         .OrderByDescending(n => n.CreatedDate);

            // Tìm kiếm nếu có từ khóa
            if (!string.IsNullOrEmpty(search))
            {
                newsQuery = newsQuery.Where(n => n.Title.Contains(search) || (n.Description != null && n.Description.Contains(search)));
                ViewData["SearchKeyword"] = search; // Lưu từ khóa tìm kiếm để hiển thị lại trên form
            }

            // Chuyển query sang danh sách
            var newsList = await newsQuery.ToListAsync(); // Dùng ToListAsync() của EF Core
            
            // Chuyển đổi xuống dòng
            newsList.ForEach(n =>
            {
                if (n.Description != null)
                {
                    n.Description = n.Description.Replace("\r\n", "<br>").Replace("\n", "<br>");
                }
            });
            // Sử dụng ToPagedList để phân trang (không bất đồng bộ)
            var pagedNews = newsList.ToPagedList(page, pageSize);

            // Lấy danh sách danh mục dịch vụ (cho header/footer)
            var categories = await _context.ServiceCategories
                                          .Include(c => c.Services)
                                          .ToListAsync();
            ViewData["ServiceCategories"] = categories;

            // Lấy danh sách bài viết gần đây (Recent Posts) cho sidebar
            var recentPosts = await _context.News
                                    .Where(n => n.Status == "Hiển thị")
                                    .OrderByDescending(n => n.CreatedDate)
                                    .Take(4) // Lấy 4 bài viết gần đây nhất
                                    .ToListAsync();
            ViewData["RecentPosts"] = recentPosts;

            return View(pagedNews);
        }

        // GET: News/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var news = await _context.News
                             .FirstOrDefaultAsync(n => n.NewsId == id && n.Status == "Hiển thị");
            if (news == null)
            {
                return NotFound();
            }

            // Lấy danh sách danh mục dịch vụ (cho header/footer)
            var categories = await _context.ServiceCategories
                                          .Include(c => c.Services)
                                          .ToListAsync();
            ViewData["ServiceCategories"] = categories;
            // Chuyển đổi xuống dòng
            if (news.Content != null)
            {
                news.Content = news.Content.Replace("\r\n", "<br>").Replace("\n", "<br>");
            }

            // Lấy danh sách bài viết gần đây (Recent Posts) cho sidebar
            var recentPosts = await _context.News
                                    .Where(n => n.Status == "Hiển thị")
                                    .OrderByDescending(n => n.CreatedDate)
                                    .Take(4)
                                    .ToListAsync();
            ViewData["RecentPosts"] = recentPosts;

            return View(news);
        }
    }
}