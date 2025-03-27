using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OfficePlantCare.Models;

namespace OfficePlantCare.Controllers
{
    public class CartsController : Controller
    {
        private readonly OfficePlantCareContext _context;

        public CartsController(OfficePlantCareContext context)
        {
            _context = context;
        }

        // GET: Carts
        public async Task<IActionResult> Index()
        {
            var categories = _context.ServiceCategories
                                     .Include(c => c.Services)
                                     .ToList();
            ViewData["ServiceCategories"] = categories;
            var customerLogin = HttpContext.Session.GetInt32("CustomerId");
            if (customerLogin == null)
            {
                ViewBag.CartEmpty = true;
                return View(new List<Cart>());
            }

            var cartItems = await _context.Carts
                     .Include(c => c.Service)
                     .Where(c => c.CustomerId == customerLogin)
                     .ToListAsync();

            ViewBag.CartEmpty = !cartItems.Any();
            return View(cartItems);
        }

        [HttpPost]
        public async Task<IActionResult> AddToCart(int serviceId, int customerId, string serviceType, string treeSize, int? numberOfTrees, string officeSize)
        {
            var service = await _context.Services.FindAsync(serviceId);
            if (service == null)
            {
                return NotFound("Không tìm thấy dịch vụ.");
            }

            if (serviceType == "Lẻ" && (!numberOfTrees.HasValue || numberOfTrees <= 0))
            {
                return BadRequest("Số lượng cây phải lớn hơn 0 khi chọn dịch vụ Lẻ.");
            }
            if (serviceType != "Lẻ" && string.IsNullOrWhiteSpace(officeSize))
            {
                return BadRequest("Diện tích văn phòng là bắt buộc khi chọn dịch vụ theo tháng.");
            }

            var servicePriceQuery = _context.ServicePrices
                .Where(p => p.ServiceId == serviceId
                         && p.ServiceType == serviceType
                         && p.TreeSize == treeSize);

            if (serviceType != "Lẻ")
            {
                servicePriceQuery = servicePriceQuery.Where(p => p.OfficeSize == officeSize);
            }

            var servicePrice = await servicePriceQuery.FirstOrDefaultAsync();

            if (servicePrice == null)
            {
                return BadRequest("Không tìm thấy giá dịch vụ phù hợp với lựa chọn của bạn.");
            }

            var existingCartItemQuery = _context.Carts
                .Where(c => c.CustomerId == customerId
                         && c.ServiceId == serviceId
                         && c.ServiceType == serviceType
                         && c.TreeSize == treeSize);

            if (serviceType != "Lẻ")
            {
                existingCartItemQuery = existingCartItemQuery.Where(c => c.OfficeSize == officeSize);
            }

            var existingCartItem = await existingCartItemQuery.FirstOrDefaultAsync();

            if (existingCartItem != null)
            {
                if (serviceType == "Lẻ")
                {
                    existingCartItem.Quantity += numberOfTrees ?? 1;
                    existingCartItem.NumberOfTrees = numberOfTrees ?? 1;
                }
                existingCartItem.Price = servicePrice.Price ?? 0;
                existingCartItem.Total = existingCartItem.Quantity * existingCartItem.Price;
            }
            else
            {
                var cartItem = new Cart
                {
                    CustomerId = customerId,
                    ServiceId = serviceId,
                    ServiceName = service.ServiceName,
                    Image = service.Image,
                    Quantity = serviceType == "Lẻ" ? (numberOfTrees ?? 1) : 1,
                    Price = servicePrice.Price ?? 0,
                    ServiceType = serviceType,
                    TreeSize = treeSize,
                    OfficeSize = serviceType == "Lẻ" ? null : officeSize,
                    NumberOfTrees = serviceType == "Lẻ" ? (numberOfTrees ?? 1) : 1,
                    Total = (servicePrice.Price ?? 0) * (serviceType == "Lẻ" ? (numberOfTrees ?? 1) : 1),
                    CreatedDate = DateTime.Now
                };
                _context.Carts.Add(cartItem);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction("Index", "Carts", new { customerId });
        }

        [HttpGet]
        public async Task<IActionResult> RemoveFromCart(int cartId)
        {
            var customerId = HttpContext.Session.GetInt32("CustomerId");

            if (customerId == null)
            {
                return RedirectToAction("Login", "Index");
            }

            var cartItem = await _context.Carts
                .FirstOrDefaultAsync(c => c.CartId == cartId && c.CustomerId == customerId.Value);

            if (cartItem == null)
            {
                return NotFound("Không tìm thấy mục trong giỏ hàng.");
            }

            _context.Carts.Remove(cartItem);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> CheckOut()
        {
            var categories = _context.ServiceCategories
                .Include(c => c.Services)
                .ToList();
            ViewData["ServiceCategories"] = categories;

            ViewBag.RoleId = HttpContext.Session.GetInt32("RoleId");
            ViewBag.PaymentMethods = _context.PaymentMethods.ToList();
            ViewBag.CustomerName = HttpContext.Session.GetString("CustomerName");
            ViewBag.CustomerId = HttpContext.Session.GetInt32("CustomerId");

            int? customerId = ViewBag.CustomerId as int?;
            int? roleId = ViewBag.RoleId as int?;
            ViewBag.Customer = _context.Customers.FirstOrDefault(c => c.CustomerId == customerId);

            if (customerId == null || roleId == null)
            {
                return RedirectToAction("Index");
            }

            var cartItems = _context.Carts
                .Include(c => c.Service)
                .Where(c => c.CustomerId == customerId)
                .ToList();

            if (!cartItems.Any())
            {
                return RedirectToAction("Index");
            }

            decimal totalAmount = 0;
            int totalQuantity = 0;

            foreach (var item in cartItems)
            {
                totalQuantity += item.Quantity;
                totalAmount += item.Quantity * item.Price;
            }

            ViewBag.totalAmount = totalAmount;

            return View(cartItems);
        }

        [HttpPost]
        public async Task<IActionResult> CheckOutPost(string address, string notes, int paymentMethodId, string scheduledDate, string scheduledTime)
        {
            var categories = _context.ServiceCategories
                .Include(c => c.Services)
                .ToList();
            ViewData["ServiceCategories"] = categories;

            ViewBag.RoleId = HttpContext.Session.GetInt32("RoleId");
            ViewBag.PaymentMethods = _context.PaymentMethods.ToList();
            ViewBag.CustomerName = HttpContext.Session.GetString("CustomerName");
            ViewBag.CustomerId = HttpContext.Session.GetInt32("CustomerId");

            int? customerId = ViewBag.CustomerId as int?;
            int? roleId = ViewBag.RoleId as int?;
            ViewBag.Customer = _context.Customers.FirstOrDefault(c => c.CustomerId == customerId);

            if (customerId == null || roleId == null)
            {
                return RedirectToAction("Index");
            }

            var cartItems = _context.Carts
                .Include(c => c.Service)
                .Where(c => c.CustomerId == customerId)
                .ToList();

            if (!cartItems.Any())
            {
                return RedirectToAction("Index");
            }

            decimal totalAmount = 0;
            int totalQuantity = 0;

            foreach (var item in cartItems)
            {
                totalQuantity += item.Quantity;
                totalAmount += item.Quantity * item.Price;
            }

            ViewBag.totalAmount = totalAmount;

            if (string.IsNullOrWhiteSpace(address) || string.IsNullOrWhiteSpace(scheduledDate) || string.IsNullOrWhiteSpace(scheduledTime))
            {
                ModelState.AddModelError("address", "Địa chỉ và thông tin ngày/giờ chăm sóc là bắt buộc.");
                ViewBag.Address = address;
                ViewBag.Notes = notes;
                ViewBag.PaymentMethodId = paymentMethodId;
                ViewBag.ScheduledDate = scheduledDate;
                ViewBag.ScheduledTime = scheduledTime;
                return View("CheckOut", cartItems);
            }

            DateTime startDate = DateTime.Parse(scheduledDate);
            TimeOnly scheduledTimeValue = TimeOnly.Parse(scheduledTime);

            var paymentMethod = await _context.PaymentMethods
                .FirstOrDefaultAsync(pm => pm.PaymentMethodId == paymentMethodId);

            int? orderId = null;
            int? requestId = null;

            if (roleId == 4) // Role 4: Tạo ServiceRequest (khách ký hợp đồng)
            {
                foreach (var item in cartItems)
                {
                    var servicePrice = await _context.ServicePrices
                        .FirstOrDefaultAsync(p => p.ServiceId == item.ServiceId
                                               && p.ServiceType == item.ServiceType
                                               && p.TreeSize == item.TreeSize
                                               && p.OfficeSize == item.OfficeSize);

                    if (servicePrice == null)
                    {
                        return BadRequest("Không tìm thấy giá dịch vụ phù hợp để tạo yêu cầu.");
                    }

                    var serviceRequest = new ServiceRequest
                    {
                        CustomerId = customerId.Value,
                        ServiceId = item.ServiceId,
                        LocationId = null,
                        PriceId = servicePrice.PriceId,
                        Quantity = item.Quantity,
                        TotalAmount = item.Total,
                        Notes = notes,
                        RequestDate = DateTime.Now,
                        UpdatedAt = DateTime.Now,
                        Status = "Chờ xử lý",
                        PaymentStatus = "Chưa thanh toán",
                        PaymentMethodId = paymentMethodId
                    };
                    _context.ServiceRequests.Add(serviceRequest);
                    await _context.SaveChangesAsync();

                    requestId = serviceRequest.RequestId;

                    if (item.ServiceType != "Lẻ")
                    {
                        int numberOfSchedules;
                        switch (item.ServiceType)
                        {
                            case "3 tháng":
                                numberOfSchedules = 24;
                                break;
                            case "6 tháng":
                                numberOfSchedules = 48;
                                break;
                            case "12 tháng":
                                numberOfSchedules = 96;
                                break;
                            default:
                                numberOfSchedules = 0;
                                break;
                        }

                        for (int i = 0; i < numberOfSchedules; i++)
                        {
                            var careSchedule = new CareSchedule
                            {
                                RequestId = serviceRequest.RequestId,
                                ContractId = null,
                                OrderId = null,
                                StaffId = null,
                                ScheduledDate = DateOnly.FromDateTime(startDate.AddDays(i * 3.5)),
                                ScheduledTime = scheduledTimeValue,
                                ActualDate = null,
                                Duration = 2.0m,
                                Status = "Chưa thực hiện"
                            };
                            _context.CareSchedules.Add(careSchedule);
                        }
                    }
                    else
                    {
                        var careSchedule = new CareSchedule
                        {
                            RequestId = serviceRequest.RequestId,
                            ContractId = null,
                            OrderId = null,
                            StaffId = null,
                            ScheduledDate = DateOnly.FromDateTime(startDate),
                            ScheduledTime = scheduledTimeValue,
                            ActualDate = null,
                            Duration = 2.0m,
                            Status = "Chưa thực hiện"
                        };
                        _context.CareSchedules.Add(careSchedule);
                    }
                }
            }
            else if (roleId == 5) // Role 5: Tạo Order và OrderDetails (khách lẻ)
            {
                var order = new Order
                {
                    CustomerId = customerId.Value,
                    OrderDate = DateTime.Now,
                    TotalPrice = totalAmount,
                    PaymentMethodId = paymentMethodId,
                    PaymentDate = null,
                    PaymentStatus = "Chưa thanh toán",
                    Status = "Chờ xử lý"
                };

                _context.Orders.Add(order);
                await _context.SaveChangesAsync();

                orderId = order.OrderId;

                foreach (var item in cartItems)
                {
                    var servicePrice = await _context.ServicePrices
                        .FirstOrDefaultAsync(p => p.ServiceId == item.ServiceId
                                               && p.ServiceType == item.ServiceType
                                               && p.TreeSize == item.TreeSize
                                               && p.OfficeSize == item.OfficeSize);

                    if (servicePrice == null)
                    {
                        return BadRequest("Không tìm thấy giá dịch vụ phù hợp để tạo đơn hàng.");
                    }

                    var orderDetail = new OrderDetail
                    {
                        OrderId = order.OrderId,
                        ServiceId = item.ServiceId,
                        Address = address,
                        Quantity = item.Quantity,
                        PriceId = servicePrice.PriceId,
                        TotalAmount = item.Total,
                        Notes = notes,
                        Status = "Đang chờ xác nhận"
                    };
                    _context.OrderDetails.Add(orderDetail);
                    await _context.SaveChangesAsync();

                    if (item.ServiceType == "Lẻ")
                    {
                        var careSchedule = new CareSchedule
                        {
                            RequestId = null,
                            ContractId = null,
                            OrderId = order.OrderId,
                            StaffId = null,
                            ScheduledDate = DateOnly.FromDateTime(startDate),
                            ScheduledTime = scheduledTimeValue,
                            ActualDate = null,
                            Duration = 2.0m,
                            Status = "Chưa thực hiện"
                        };
                        _context.CareSchedules.Add(careSchedule);
                    }
                    else
                    {
                        int numberOfSchedules;
                        switch (item.ServiceType)
                        {
                            case "3 tháng":
                                numberOfSchedules = 24;
                                break;
                            case "6 tháng":
                                numberOfSchedules = 48;
                                break;
                            case "12 tháng":
                                numberOfSchedules = 96;
                                break;
                            default:
                                numberOfSchedules = 0;
                                break;
                        }

                        for (int i = 0; i < numberOfSchedules; i++)
                        {
                            var careSchedule = new CareSchedule
                            {
                                RequestId = null,
                                ContractId = null,
                                OrderId = order.OrderId,
                                StaffId = null,
                                ScheduledDate = DateOnly.FromDateTime(startDate.AddDays(i * 3.5)),
                                ScheduledTime = scheduledTimeValue,
                                ActualDate = null,
                                Duration = 2.0m,
                                Status = "Chưa thực hiện"
                            };
                            _context.CareSchedules.Add(careSchedule);
                        }
                    }
                }
            }

            await _context.SaveChangesAsync();

            _context.Carts.RemoveRange(cartItems);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Success), new { orderId, requestId, totalAmount, address, paymentDate = DateTime.Now, paymentStatus = "Chưa thanh toán" });
        }

        public IActionResult Success(int? orderId, int? requestId, decimal totalAmount, string address, DateTime paymentDate, string paymentStatus)
        {
            ViewBag.OrderId = orderId ?? requestId; // Sử dụng orderId hoặc requestId làm mã đơn hàng
            ViewBag.TotalAmount = totalAmount;
            ViewBag.Address = address;
            ViewBag.PaymentDate = paymentDate.ToString("dd/MM/yyyy HH:mm:ss");
            ViewBag.PaymentStatus = paymentStatus;

            return View();
        }
    }
}