using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using OfficePlantCare.Models;

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
                                         .Include(c => c.Services) // N?p luôn danh sách d?ch v? thu?c danh m?c ?ó
                                         .ToList();

            ViewData["ServiceCategories"] = categories; // Truy?n vào ViewData
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

            var dataLogin = _context.Customers
                .Where(x => x.Email == model.Email && x.PasswordHash == model.PasswordHash)
                .FirstOrDefault();

            if (dataLogin != null)
            {
                // Serialize đối tượng Customer và lưu vào session
                string customerJson = JsonConvert.SerializeObject(dataLogin);
                HttpContext.Session.SetString("CustomerLogin", customerJson);
                HttpContext.Session.SetInt32("CustomerId", dataLogin.CustomerId);
                HttpContext.Session.SetInt32("RoleId", (int)dataLogin.RoleId); // Lưu RoleId vào session
                                                                               // Điều hướng đến Dashboard theo role
                if (dataLogin.RoleId == 4) // Hợp đồng
                {
                    return RedirectToAction("Index", "Home");
                }
                else if (dataLogin.RoleId == 5) // Đặt dịch vụ lẻ
                {
                    return RedirectToAction("Index", "Home");
                }
            }
            return View(model); // Nếu không đăng nhập thành công, trả về trang đăng nhập
        }


        [HttpGet]
        public IActionResult Logout()
        {
            HttpContext.Session.Remove("CustomerLogin");
            HttpContext.Session.Remove("CustomerId");

            return RedirectToAction("Index");
        }
    }
}
