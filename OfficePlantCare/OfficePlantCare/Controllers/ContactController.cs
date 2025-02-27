using Microsoft.AspNetCore.Mvc;
using OfficePlantCare.Models;
using Microsoft.EntityFrameworkCore;

namespace OfficePlantCare.Controllers
{
    public class ContactController : Controller
    {
        private readonly OfficePlantCareContext _context;
        public ContactController(OfficePlantCareContext context)
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
