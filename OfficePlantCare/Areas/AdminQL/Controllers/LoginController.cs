using Microsoft.AspNetCore.Mvc;
using OfficePlantCare.Models;
using OfficePlantCare.Areas.AdminQL.Models;
using System.Security.Cryptography;
using System.Text;

namespace OfficePlantCare.Areas.AdminQL.Controllers
{
    [Area("AdminQL")]
    public class LoginController : Controller
    {
        private readonly OfficePlantCareContext _context;

        public LoginController(OfficePlantCareContext context)
        {
            _context = context;
        }

        [HttpGet] // Hiển thị form đăng nhập
        public IActionResult Index()
        {
            // Kiểm tra session để tránh redirect lặp
            if (HttpContext.Session.GetInt32("AdminId") != null)
            {
                return RedirectToAction("AdminDashboard", "Dashboard");
            }
            return View();
        }

        [HttpPost]
        public IActionResult Index(Login model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var pass = GetSHA256Hash(model.Password);
            var dataLogin = _context.Admins
                .Where(x => x.Email.Equals(model.Email) && x.PasswordHash.Equals(pass))
                .FirstOrDefault();

            if (dataLogin != null)
            {
                // Lưu thông tin vào Session
                HttpContext.Session.SetString("AdminLogin", model.Email);
                HttpContext.Session.SetString("AdminName", dataLogin.Username);
                HttpContext.Session.SetString("AdminAddress", dataLogin.Address);
                HttpContext.Session.SetInt32("AdminId", dataLogin.AdminId);
                HttpContext.Session.SetInt32("RoleId", (int)dataLogin.RoleId);

                // Chuyển hướng dựa trên RoleId
                if (dataLogin.RoleId == 1)
                {
                    return RedirectToAction("AdminDashboard", "Dashboard");
                }
                else if (dataLogin.RoleId == 2)
                {
                    return RedirectToAction("SalesDashboard", "Dashboard");
                }
            }

            ModelState.AddModelError("", "Email hoặc mật khẩu không đúng.");
            return View(model);
        }

        [HttpGet] // Thoát đăng nhập
        public IActionResult Logout()
        {
            // Xóa tất cả session
            HttpContext.Session.Clear();
            return RedirectToAction("Index");
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