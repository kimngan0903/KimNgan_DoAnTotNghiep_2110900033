using Microsoft.AspNetCore.Mvc;
using PlantCare.Areas.AdminQL.Models;
using PlantCare.Models;

namespace PlantCare.Areas.AdminQL.Controllers
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
            return View();
        }

        [HttpPost] // Xử lý khi người dùng submit form đăng nhập
        public IActionResult Index(Login model)
        {
            if (!ModelState.IsValid)
            {
                return View(model); // Trả về trạng thái lỗi nếu form không hợp lệ
            }

            // Xử lý logic đăng nhập
            var pass = model.Password;
            var dataLogin = _context.Admins
                .Where(x => x.Email.Equals(model.Email) && x.PasswordHash.Equals(pass))
                .FirstOrDefault();

            if (dataLogin != null)
            {
                // Lưu session khi đăng nhập thành công
                HttpContext.Session.SetString("AdminLogin", model.Email);
                HttpContext.Session.SetString("AdminName", dataLogin.Username);
                HttpContext.Session.SetString("AdminAddress", dataLogin.Address);
                HttpContext.Session.SetInt32("AdminId", dataLogin.AdminId);
                HttpContext.Session.SetInt32("RoleId", (int)dataLogin.RoleId); // Lưu RoleId vào session

                // Điều hướng đến Dashboard theo role
                if (dataLogin.RoleId == 1) // Admin
                {
                    return RedirectToAction("AdminDashboard", "Dashboard");
                }
                else if (dataLogin.RoleId == 2) // Sales
                {
                    return RedirectToAction("SalesDashboard", "Dashboard");
                }
            }

            return View(model); // Nếu không đăng nhập thành công, trả về trang đăng nhập
        }

        [HttpGet] // Thoát đăng nhập, xóa session
        public IActionResult Logout()
        {
            HttpContext.Session.Remove("AdminLogin"); // Xóa session với key AdminLogin
            HttpContext.Session.Remove("RoleId"); // Xóa session với key RoleId

            return RedirectToAction("Index"); // Chuyển hướng về trang đăng nhập
        }
    }
}