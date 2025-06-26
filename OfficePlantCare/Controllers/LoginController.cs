using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using OfficePlantCare.Models;
using System.Security.Cryptography;
using System.Text;

namespace OfficePlantCare.Controllers
{
    public class LoginController : Controller
    {
        private readonly OfficePlantCareContext _context;

        public LoginController(OfficePlantCareContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var categories = _context.ServiceCategories
                                     .Include(c => c.Services)
                                     .ToList();
            ViewData["ServiceCategories"] = categories;

            // Kiểm tra session để tránh redirect lặp
            var customerId = HttpContext.Session.GetInt32("CustomerId");
            if (customerId != null)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        [HttpPost]
        public IActionResult Index(LoginUser model)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError(string.Empty, "Thông tin đăng nhập không hợp lệ.");
                return View(model);
            }

            var pass = GetSHA256Hash(model.PasswordHash);
            var dataLogin = _context.Customers
                .Where(x => x.Email == model.Email && x.PasswordHash == pass)
                .FirstOrDefault();

            if (dataLogin != null)
            {
                // Lưu thông tin vào Session, thêm cờ đăng nhập
                HttpContext.Session.SetInt32("CustomerId", dataLogin.CustomerId);
                HttpContext.Session.SetString("IsLoggedIn", "true"); // Thêm cờ để kiểm tra đăng nhập             
                HttpContext.Session.SetString("CustomerName", dataLogin.CustomerName);
                return RedirectToAction("Index", "Home");
            }

            // Đăng nhập thất bại
            ModelState.AddModelError(string.Empty, "Email hoặc mật khẩu không đúng.");
            return View(model);
        }

        [HttpGet]
        public IActionResult Logout()
        {
            // Xóa tất cả session
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Login");
        }

        static string GetSHA256Hash(string input)
        {
            using (var sha256 = SHA256.Create())
            {
                var hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(input));
                return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
            }
        }
    }
}
