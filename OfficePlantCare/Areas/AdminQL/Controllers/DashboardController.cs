using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OfficePlantCare.Areas.AdminQL.Models;
using OfficePlantCare.Models;
using System;
using X.PagedList;
using X.PagedList.Extensions;

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

        // Action để cập nhật trạng thái thông báo (đánh dấu tất cả là đã đọc hoặc xóa tất cả)
        [HttpPost]
        public IActionResult UpdateNotifications(string action, int adminId)
        {
            if (adminId == 0) return RedirectToAction("Index", "Login");

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

        // Action để xóa các thông báo đã chọn
        [HttpPost]
        public IActionResult DeleteNotifications(int[] notificationIds, int page = 1, string keyword = null, string type = null, string status = null)
        {
            var adminId = _httpContextAccessor.HttpContext.Session.GetInt32("AdminId") ?? 0;
            if (adminId == 0) return RedirectToAction("Index", "Login");

            if (notificationIds != null && notificationIds.Length > 0)
            {
                var notificationsToDelete = _context.Notifications
                    .Where(n => notificationIds.Contains(n.NotificationId) && n.AdminId == adminId && !n.IsDeleted)
                    .ToList();

                foreach (var notification in notificationsToDelete)
                {
                    notification.IsDeleted = true;
                }

                _context.SaveChanges();
            }

            return RedirectToAction("AllNotifications", new { page, keyword, type, status });
        }

        // Action để chuyển trạng thái đọc/chưa đọc
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ToggleReadStatus(int notificationId, bool isRead, int page = 1, string keyword = null, string type = null, string status = null)
        {
            var adminId = _httpContextAccessor.HttpContext.Session.GetInt32("AdminId") ?? 0;
            if (adminId == 0)
            {
                return RedirectToAction("Index", "Login");
            }

            var notification = _context.Notifications
                .FirstOrDefault(n => n.NotificationId == notificationId && n.AdminId == adminId && !n.IsDeleted);

            if (notification != null)
            {
                notification.IsRead = isRead;
                _context.SaveChanges();
            }

            return RedirectToAction("AllNotifications", new { page, keyword, type, status });
        }
        // Action hiển thị danh sách thông báo với phân trang và bộ lọc
        public IActionResult AllNotifications(int? page, string keyword, string type, string status)
        {
            var adminId = _httpContextAccessor.HttpContext.Session.GetInt32("AdminId") ?? 0;
            if (adminId == 0) return RedirectToAction("Index", "Login");

            var pageNumber = page ?? 1;
            var pageSize = 10;

            var query = _context.Notifications
                .Where(n => n.AdminId == adminId && !n.IsDeleted);

            // Lọc theo từ khóa
            if (!string.IsNullOrEmpty(keyword))
            {
                query = query.Where(n =>
                    n.Message.Contains(keyword) ||
                    n.Type.Contains(keyword) ||
                    (_context.Orders.Any(o => o.OrderId.ToString() == n.ReferenceId && n.Type == "Đơn hàng" &&
                        (o.CustomerId.HasValue && _context.Customers.Any(c => c.CustomerId == o.CustomerId && c.CustomerName.Contains(keyword))))) ||
                    (_context.Feedbacks.Any(f => f.FeedbackId.ToString() == n.ReferenceId && n.Type == "Phản hồi" &&
                        (f.CustomerId.HasValue && _context.Customers.Any(c => c.CustomerId == f.CustomerId && c.CustomerName.Contains(keyword))))) ||
                    (_context.Contracts.Any(ct => ct.ContractId.ToString() == n.ReferenceId && n.Type == "Hợp đồng" && _context.Customers.Any(c => c.CustomerId == ct.CustomerId
                        && c.CustomerName.Contains(keyword))))
                );
            }

            // Lọc theo loại thông báo
            if (!string.IsNullOrEmpty(type))
            {
                query = query.Where(n => n.Type == type);
            }

            // Lọc theo trạng thái
            if (!string.IsNullOrEmpty(status) && status != "all")
            {
                bool isRead = status == "read";
                query = query.Where(n => n.IsRead == isRead);
            }

            var rawNotifications = query
                .OrderByDescending(n => n.CreatedDate)
                .Select(n => new
                {
                    n.NotificationId,
                    n.Type,
                    n.Message,
                    n.CreatedDate,
                    n.IsRead,
                    n.IsDeleted,
                    n.ReferenceId
                })
                .ToPagedList(pageNumber, pageSize);

            var notifications = new StaticPagedList<NotificationViewModel>(
                rawNotifications.Select(n => new NotificationViewModel
                {
                    NotificationId = n.NotificationId,
                    Type = n.Type,
                    Message = n.Message,
                    TimeAgo = CalculateTimeAgo(n.CreatedDate),
                    IsRead = n.IsRead,
                    IsDeleted = n.IsDeleted,
                    Icon = GetNotificationIcon(n.Type),
                    Link = GetNotificationLink(n.Type, n.ReferenceId)
                }),
                rawNotifications.PageNumber,
                rawNotifications.PageSize,
                rawNotifications.TotalItemCount
            );

            ViewBag.keyword = keyword;
            ViewBag.type = type;
            ViewBag.status = status;
            return View("AllNotifications", notifications);
        }

        [HttpPost]
        public IActionResult CreateOrder(Order model)
        {
            var adminId = _httpContextAccessor.HttpContext.Session.GetInt32("AdminId") ?? 0;
            if (adminId == 0) return RedirectToAction("Index", "Login");

            if (ModelState.IsValid)
            {
                model.OrderDate = DateTime.Now;
                model.Status = "Chờ xử lý";
                _context.Orders.Add(model);
                _context.SaveChanges();

                var customerName = model.CustomerId.HasValue ? _context.Customers.Find(model.CustomerId)?.CustomerName : "Khách lẻ";
                AddNotification(adminId, model.OrderId.ToString(), "Đơn hàng", $"Đơn hàng mới #{model.OrderId} từ {customerName}");

                return RedirectToAction("Index");
            }
            return View(model);
        }

        [HttpPost]
        public IActionResult CreateFeedback(Feedback model)
        {
            var adminId = _httpContextAccessor.HttpContext.Session.GetInt32("AdminId") ?? 0;
            if (adminId == 0) return RedirectToAction("Index", "Login");

            if (ModelState.IsValid)
            {
                model.FeedbackDate = DateTime.Now;
                _context.Feedbacks.Add(model);
                _context.SaveChanges();

                var customerName = model.CustomerId.HasValue ? _context.Customers.Find(model.CustomerId)?.CustomerName : "Khách ẩn danh";
                AddNotification(adminId, model.FeedbackId.ToString(), "Phản hồi", $"Phản hồi mới từ {customerName}");

                return RedirectToAction("Index");
            }
            return View(model);
        }

        [HttpPost]
        public IActionResult CreateContract(Contract model)
        {
            var adminId = _httpContextAccessor.HttpContext.Session.GetInt32("AdminId") ?? 0;
            if (adminId == 0) return RedirectToAction("Index", "Login");

            if (ModelState.IsValid)
            {
                model.StartDate = DateOnly.FromDateTime(DateTime.Now);
                model.Status = "Chờ duyệt";
                _context.Contracts.Add(model);
                _context.SaveChanges();

                var customerName = _context.Customers.Find(model.CustomerId)?.CustomerName ?? "Khách không xác định";
                AddNotification(adminId, model.ContractId.ToString(), "Hợp đồng", $"Hợp đồng mới #{model.ContractCode} từ {customerName}");

                return RedirectToAction("Index");
            }
            return View(model);
        }

        private void AddNotification(int adminId, string referenceId, string type, string message)
        {
            if (adminId == 0) return;

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
            Console.WriteLine($"Added notification: {message} for AdminId: {adminId}");
        }

        private string CalculateTimeAgo(DateTime dateTime)
        {
            var timeSpan = DateTime.Now - dateTime;
            if (timeSpan.TotalMinutes < 1) return "Vừa xong";
            if (timeSpan.TotalHours < 1) return $"{(int)timeSpan.TotalMinutes} phút trước";
            if (timeSpan.TotalDays < 1) return $"{(int)timeSpan.TotalHours} giờ trước";
            return $"{(int)timeSpan.TotalDays} ngày trước";
        }

        private string GetNotificationIcon(string type)
        {
            return type switch
            {
                "Đơn hàng" => "fas fa-shopping-cart",
                "Yêu cầu phát sinh" => "fas fa-exclamation-circle",
                "Phản hồi" => "fas fa-comment-dots",
                "Hợp đồng" => "fas fa-file-contract",
                _ => "fas fa-bell"
            };
        }

        private string GetNotificationLink(string type, string referenceId)
        {
            return type switch
            {
                "Đơn hàng" => Url.Action("Details", "Orders", new { id = referenceId, area = "AdminQL" }),
                "Yêu cầu phát sinh" => Url.Action("Details", "ServiceRequests", new { id = referenceId, area = "AdminQL" }),
                "Phản hồi" => Url.Action("Details", "Feedbacks", new { id = referenceId, area = "AdminQL" }),
                "Hợp đồng" => Url.Action("Details", "Contracts", new { id = referenceId, area = "AdminQL" }),
                _ => null
            };
        }
    }
}