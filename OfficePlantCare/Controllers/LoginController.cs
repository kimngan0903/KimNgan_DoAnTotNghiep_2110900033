using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using OfficePlantCare.Models;
using System.Diagnostics.Contracts;

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
                // Lưu thông tin khách hàng vào Session
                string customerJson = JsonConvert.SerializeObject(dataLogin);
                HttpContext.Session.SetString("CustomerLogin", customerJson);
                HttpContext.Session.SetInt32("CustomerId", dataLogin.CustomerId);
                HttpContext.Session.SetInt32("RoleId", (int)dataLogin.RoleId);

                if (dataLogin.RoleId == 4) // Khách ký hợp đồng
                {
                    var contract = _context.Contracts.FirstOrDefault(c => c.CustomerId == dataLogin.CustomerId);
                    if (contract != null)
                    {
                        HttpContext.Session.SetString("ContractCode", contract.ContractCode);
                    }
                    return RedirectToAction("Index", "Home");
                }
                else if (dataLogin.RoleId == 5) // Khách đặt dịch vụ
                {
                    HttpContext.Session.SetString("CustomerName", dataLogin.CustomerName);
                    return RedirectToAction("Index", "Home");
                }
            }

            // Đăng nhập thất bại
            ModelState.AddModelError(string.Empty, "Email hoặc mật khẩu không đúng.");
            return View(model);
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
