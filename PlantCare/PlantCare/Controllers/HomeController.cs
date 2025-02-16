using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PlantCare.Models;
using System.Diagnostics;

namespace PlantCare.Controllers
{
    public class HomeController : Controller
    {
        private readonly OfficePlantCareContext _context;
        private readonly ILogger<HomeController> _logger;

        public HomeController(OfficePlantCareContext context, ILogger<HomeController> logger)
        {
            _context = context;
            _logger = logger;
        }

        public IActionResult Index()
        {
            var categories = _context.ServiceCategories
                               .Include(c => c.Services) // N?p luôn danh sách d?ch v? thu?c danh m?c ?ó
                               .ToList();

            ViewData["ServiceCategories"] = categories; // Truy?n vào ViewData

            return View();
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
