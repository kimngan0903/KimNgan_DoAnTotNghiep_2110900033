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
        public IActionResult Index()
        {
            var categories = _context.ServiceCategories
                                          .Include(c => c.Services) // N?p luôn danh sách d?ch v? thu?c danh m?c ?ó
                                          .ToList();

            ViewData["ServiceCategories"] = categories; // Truy?n vào ViewData

            return View();
        }
    }
}
