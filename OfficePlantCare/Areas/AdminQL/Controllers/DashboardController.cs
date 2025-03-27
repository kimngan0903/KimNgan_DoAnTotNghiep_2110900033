using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OfficePlantCare.Areas.AdminQL.Models;
using OfficePlantCare.Models;
using System;

namespace OfficePlantCare.Areas.AdminQL.Controllers
{
    public class DashboardController : BaseController
    {
        private readonly DashboardService _dashboardService;
        private readonly OfficePlantCareContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public DashboardController(DashboardService dashboardService, OfficePlantCareContext context, IHttpContextAccessor httpContextAccessor)
        {
            _dashboardService = dashboardService;
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        // Đây là action chung cho tất cả các dashboard
        public IActionResult Index()
        {
            var roleId = HttpContext.Session.GetInt32("RoleId"); // Lấy RoleId từ session

            if (roleId == null)
            {
                return RedirectToAction("Index", "Login"); // Nếu không có roleId trong session, điều hướng đến trang đăng nhập
            }

            // Điều hướng người dùng đến dashboard dựa trên roleId
            if (roleId == 1) // Admin
            {
                return RedirectToAction("AdminDashboard");
            }
            else if (roleId == 2) // Sales
            {
                return RedirectToAction("SalesDashboard");
            }

            return RedirectToAction("Index", "Login"); // Nếu role không hợp lệ, quay lại đăng nhập
        }

        // Action dành cho Admin
        public IActionResult AdminDashboard()
        {
            var roleId = HttpContext.Session.GetInt32("RoleId");
            if (roleId != 1) // Nếu không phải Admin, redirect về trang đăng nhập
                return RedirectToAction("Index", "Login");

            var data = _dashboardService.GetDashboardData(); // Lấy dữ liệu cho Admin
            return View("AdminDashboard", data); // Truyền dữ liệu vào view AdminDashboard
        }

        // Action dành cho Sales
        public IActionResult SalesDashboard()
        {
            var roleId = HttpContext.Session.GetInt32("RoleId");
            if (roleId != 2) // Nếu không phải Sales, redirect về trang đăng nhập
                return RedirectToAction("Index", "Login");

            var data = _dashboardService.GetDashboardData(); // Lấy dữ liệu cho Sales
            return View("SalesDashboard", data); // Truyền dữ liệu vào view SalesDashboard
        }
        // Action để cập nhật trạng thái thông báo
        [HttpPost]
        public IActionResult UpdateNotifications(string action, int adminId)
        {
            if (adminId == 0) return RedirectToAction("Index");

            var notifications = _context.Notifications
                .Where(n => n.AdminId == adminId && !n.IsDeleted)
                .ToList();

            if (action == "markAllRead")
            {
                foreach (var notification in notifications.Where(n => !n.IsRead))
                {
                    notification.IsRead = true;
                }
            }
            else if (action == "clearAll")
            {
                foreach (var notification in notifications)
                {
                    notification.IsDeleted = true;
                }
            }

            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        // Action để hiển thị tất cả thông báo
        public IActionResult AllNotifications()
        {
            var adminId = _httpContextAccessor.HttpContext.Session.GetInt32("AdminId") ?? 0;
            if (adminId == 0)
            {
                return RedirectToAction("Index", "Login");
            }

            var notifications = _context.Notifications
                .Where(n => n.AdminId == adminId && !n.IsDeleted)
                .OrderByDescending(n => n.CreatedDate)
                .Select(n => new NotificationViewModel
                {
                    NotificationId = n.NotificationId,
                    Type = n.Type,
                    Message = n.Message,
                    TimeAgo = CalculateTimeAgo(n.CreatedDate),
                    IsRead = n.IsRead,
                    IsDeleted = n.IsDeleted,
                    Icon = GetNotificationIcon(n.Type),
                    Link = GetNotificationLink(n.Type, n.ReferenceId)
                })
                .ToList();

            return View("AllNotifications", notifications);
        }

        // Action tạo đơn hàng và thêm thông báo
        [HttpPost]
        public IActionResult CreateOrder(Order model)
        {
            if (ModelState.IsValid)
            {
                model.OrderDate = DateTime.Now;
                model.Status = "Chờ xử lý";
                _context.Orders.Add(model);
                _context.SaveChanges();

                var adminId = _httpContextAccessor.HttpContext.Session.GetInt32("AdminId") ?? 0;
                var customerName = model.CustomerId.HasValue ? _context.Customers.Find(model.CustomerId)?.CustomerName : "Khách lẻ";
                AddNotification(adminId, model.OrderId.ToString(), "Order", $"Đơn hàng mới #{model.OrderId} từ {customerName}");

                return RedirectToAction("Index");
            }
            return View(model);
        }

        // Action tạo yêu cầu phát sinh và thêm thông báo
        [HttpPost]
        public IActionResult CreateServiceRequest(ServiceRequest model)
        {
            if (ModelState.IsValid)
            {
                model.RequestDate = DateTime.Now;
                model.Status = "Chờ xử lý";
                _context.ServiceRequests.Add(model);
                _context.SaveChanges();

                var adminId = _httpContextAccessor.HttpContext.Session.GetInt32("AdminId") ?? 0;
                var customerName = _context.Customers.Find(model.CustomerId)?.CustomerName ?? "Ký hợp đồng";
                AddNotification(adminId, model.RequestId.ToString(), "ServiceRequest", $"Yêu cầu phát sinh #{model.RequestId} từ {customerName}");

                return RedirectToAction("Index");
            }
            return View(model);
        }

        // Action tạo liên hệ và thêm thông báo
        [HttpPost]
        public IActionResult CreateContact(Contact model)
        {
            if (ModelState.IsValid)
            {
                model.CreatedDate = DateTime.Now;
                model.Status = "Chưa xử lý";
                _context.Contacts.Add(model);
                _context.SaveChanges();

                var adminId = _httpContextAccessor.HttpContext.Session.GetInt32("AdminId") ?? 0;
                AddNotification(adminId, model.ContactId.ToString(), "Contact", $"Liên hệ mới từ {model.Email ?? model.Phone}");

                return RedirectToAction("Index");
            }
            return View(model);
        }

        // Action tạo phản hồi và thêm thông báo
        [HttpPost]
        public IActionResult CreateFeedback(Feedback model)
        {
            if (ModelState.IsValid)
            {
                model.FeedbackDate = DateTime.Now;
                model.Status = "Chờ duyệt";
                _context.Feedbacks.Add(model);
                _context.SaveChanges();

                var adminId = _httpContextAccessor.HttpContext.Session.GetInt32("AdminId") ?? 0;
                var customerName = model.CustomerId.HasValue ? _context.Customers.Find(model.CustomerId)?.CustomerName : "Khách ẩn danh";
                AddNotification(adminId, model.FeedbackId.ToString(), "Feedback", $"Phản hồi mới từ {customerName}");

                return RedirectToAction("Index");
            }
            return View(model);
        }

        // Phương thức thêm thông báo vào bảng Notifications
        private void AddNotification(int adminId, string referenceId, string type, string message)
        {
            if (adminId == 0) return;

            var existingNotification = _context.Notifications
                .FirstOrDefault(n => n.AdminId == adminId && n.Type == type && n.ReferenceId == referenceId);

            if (existingNotification == null)
            {
                var notification = new Notification
                {
                    AdminId = adminId,
                    ReferenceId = referenceId,
                    Type = type,
                    Message = message,
                    CreatedDate = DateTime.Now,
                    IsRead = false,
                    IsDeleted = false
                };
                _context.Notifications.Add(notification);
                _context.SaveChanges();
            }
        }

        // Phương thức tính thời gian cách đây
        private string CalculateTimeAgo(DateTime dateTime)
        {
            var timeSpan = DateTime.Now - dateTime;
            if (timeSpan.TotalMinutes < 1) return "Vừa xong";
            if (timeSpan.TotalHours < 1) return $"{(int)timeSpan.TotalMinutes} phút trước";
            if (timeSpan.TotalDays < 1) return $"{(int)timeSpan.TotalHours} giờ trước";
            return $"{(int)timeSpan.TotalDays} ngày trước";
        }

        // Phương thức lấy icon thông báo
        private string GetNotificationIcon(string type)
        {
            return type switch
            {
                "Order" => "fas fa-shopping-cart",
                "ServiceRequest" => "fas fa-exclamation-circle",
                "Contact" => "fas fa-address-book",
                "Feedback" => "fas fa-comment-dots",
                _ => "fas fa-bell"
            };
        }

        // Phương thức lấy liên kết thông báo
        private string GetNotificationLink(string type, string referenceId)
        {
            return type switch
            {
                "Order" => Url.Action("Detail", "Orders", new { id = referenceId, area = "AdminQL" }),
                "ServiceRequest" => Url.Action("Detail", "ServiceRequests", new { id = referenceId, area = "AdminQL" }),
                "Contact" => Url.Action("Detail", "Contacts", new { id = referenceId, area = "AdminQL" }),
                "Feedback" => Url.Action("Detail", "Feedbacks", new { id = referenceId, area = "AdminQL" }),
                _ => null
            };
        }
    }
}
