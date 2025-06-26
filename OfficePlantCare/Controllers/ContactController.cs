using Microsoft.AspNetCore.Mvc;
using OfficePlantCare.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace OfficePlantCare.Controllers
{
    public class ContactController : Controller
    {
        private readonly OfficePlantCareContext _context;

        public ContactController(OfficePlantCareContext context)
        {
            _context = context;
        }

        // GET: Contact
        public IActionResult Index()
        {
            var categories = _context.ServiceCategories
                                     .Include(c => c.Services)
                                     .ToList();

            ViewData["ServiceCategories"] = categories;
            return View();
        }

        // POST: Contact/SubmitContact
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SubmitContact(string Name, string Email, string Phone, string Address, string Message)
        {
            if (ModelState.IsValid)
            {
                // Tạo một bản ghi Contact mới
                var contact = new Contact
                {
                    Email = Email,
                    Phone = Phone,
                    Address = Address, 
                    Description = $"Tên: {Name} - Nội dung: {Message}", 
                    CreatedDate = DateTime.Now,
                };

                // Thêm vào cơ sở dữ liệu
                _context.Contacts.Add(contact);
                await _context.SaveChangesAsync();

                // Thêm thông báo thành công vào TempData
                TempData["SuccessMessage"] = "Yêu cầu liên hệ của bạn đã được gửi thành công! Chúng tôi sẽ liên hệ với bạn sớm nhất.";

                // Chuyển hướng về trang Index
                return RedirectToAction(nameof(Index));
            }

            // Nếu có lỗi, trả lại trang Index với dữ liệu hiện tại
            var categories = _context.ServiceCategories
                                     .Include(c => c.Services)
                                     .ToList();
            ViewData["ServiceCategories"] = categories;
            return View("Index");
        }
    }
}