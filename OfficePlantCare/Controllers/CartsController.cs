using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
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
            var categories = await _context.ServiceCategories
                .Include(c => c.Services)
                .ToListAsync();
            ViewData["ServiceCategories"] = categories;

            var customerId = HttpContext.Session.GetInt32("CustomerId");
            if (customerId == null)
            {
                ViewBag.CartEmpty = true;
                return View(new List<Cart>());
            }

            var cartItems = await _context.Carts
                .Include(c => c.Service)
                .Where(c => c.CustomerId == customerId)
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
                .Where(p => p.ServiceId == serviceId && p.ServiceType == serviceType && p.TreeSize == treeSize);

            if (serviceType != "Lẻ")
            {
                servicePriceQuery = servicePriceQuery.Where(p => p.OfficeSize == officeSize);
            }

            var servicePrice = await servicePriceQuery.FirstOrDefaultAsync();
            if (servicePrice == null)
            {
                return BadRequest("Không tìm thấy giá dịch vụ phù hợp với lựa chọn của bạn.");
            }

            var existingCartItem = await _context.Carts
                .Where(c => c.CustomerId == customerId && c.ServiceId == serviceId && c.ServiceType == serviceType && c.TreeSize == treeSize)
                .FirstOrDefaultAsync();

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

            return RedirectToAction(serviceType == "Lẻ" ? "CartIndex" : "ContractIndex");
        }

        // Cart for individual services
        public async Task<IActionResult> CartIndex()
        {
            var categories = await _context.ServiceCategories
                .Include(c => c.Services)
                .ToListAsync();
            ViewData["ServiceCategories"] = categories;
            var customerId = HttpContext.Session.GetInt32("CustomerId");
            if (customerId == null)
            {
                ViewBag.CartEmpty = true;
                return View("CartIndex", new List<Cart>());
            }

            var cartItems = await _context.Carts
                .Include(c => c.Service)
                .Where(c => c.CustomerId == customerId && c.ServiceType == "Lẻ")
                .ToListAsync();

            ViewBag.CartEmpty = !cartItems.Any();
            return View("CartIndex", cartItems);
        }

        // Contract index for monthly services
        public async Task<IActionResult> ContractIndex()
        {
            var categories = await _context.ServiceCategories
                .Include(c => c.Services)
                .ToListAsync();
            ViewData["ServiceCategories"] = categories;
            var customerId = HttpContext.Session.GetInt32("CustomerId");
            if (customerId == null)
            {
                ViewBag.CartEmpty = true;
                return View("ContractIndex", new List<Cart>());
            }

            var contractItems = await _context.Carts
                .Include(c => c.Service)
                .Where(c => c.CustomerId == customerId && c.ServiceType != "Lẻ")
                .ToListAsync();

            ViewBag.CartEmpty = !contractItems.Any();
            return View("ContractIndex", contractItems);
        }

        public async Task<IActionResult> CheckOutCart()
        {
            var categories = await _context.ServiceCategories
                .Include(c => c.Services)
                .ToListAsync();
            ViewData["ServiceCategories"] = categories;
            var customerId = HttpContext.Session.GetInt32("CustomerId");
            if (customerId == null)
            {
                return RedirectToAction("Index", "Login");
            }

            var cartItems = await _context.Carts
                .Include(c => c.Service)
                .Where(c => c.CustomerId == customerId && c.ServiceType == "Lẻ")
                .ToListAsync();

            if (!cartItems.Any())
            {
                return RedirectToAction("CartIndex");
            }

            decimal totalAmount = cartItems.Sum(item => item.Quantity * item.Price);
            ViewBag.totalAmount = totalAmount;
            ViewBag.CustomerId = customerId;
            ViewBag.PaymentMethods = await _context.PaymentMethods.ToListAsync();
            ViewBag.Customer = await _context.Customers.FirstOrDefaultAsync(c => c.CustomerId == customerId);
            ViewBag.CustomerName = HttpContext.Session.GetString("CustomerName");

            return View("CheckOutCart", cartItems);
        }

        public async Task<IActionResult> CheckOutContract()
        {
            var categories = await _context.ServiceCategories
                .Include(c => c.Services)
                .ToListAsync();
            ViewData["ServiceCategories"] = categories;
            var customerId = HttpContext.Session.GetInt32("CustomerId");
            if (customerId == null)
            {
                return RedirectToAction("Index", "Login");
            }

            var contractItems = await _context.Carts
                .Include(c => c.Service)
                .Where(c => c.CustomerId == customerId && c.ServiceType != "Lẻ")
                .ToListAsync();

            if (!contractItems.Any())
            {
                return RedirectToAction("ContractIndex");
            }

            decimal totalAmount = contractItems.Sum(item => item.Quantity * item.Price);
            ViewBag.totalAmount = totalAmount;
            ViewBag.CustomerId = customerId;
            ViewBag.PaymentMethods = await _context.PaymentMethods.ToListAsync();
            ViewBag.Customer = await _context.Customers.FirstOrDefaultAsync(c => c.CustomerId == customerId);
            ViewBag.CustomerName = HttpContext.Session.GetString("CustomerName");

            return View("CheckOutContract", contractItems);
        }

        [HttpPost]
        public async Task<IActionResult> CheckOutPost(string address, string notes, int paymentMethodId, string scheduledDate1, string scheduledTime1, string scheduledDate2 = null, string scheduledTime2 = null, string scheduleOption = null, string checkoutType = "Cart")
        {
            var categories = await _context.ServiceCategories
                .Include(c => c.Services)
                .ToListAsync();
            ViewData["ServiceCategories"] = categories;
            var customerId = HttpContext.Session.GetInt32("CustomerId");
            if (customerId == null)
            {
                return RedirectToAction("Index");
            }

            var cartItems = await _context.Carts
                .Include(c => c.Service)
                .Where(c => c.CustomerId == customerId)
                .ToListAsync();

            if (!cartItems.Any())
            {
                return RedirectToAction(checkoutType == "Carts" ? "CartIndex" : "ContractIndex");
            }

            // Tính totalAmount dựa trên checkoutType
            decimal totalAmount = checkoutType == "Carts"
                ? cartItems.Where(c => c.ServiceType == "Lẻ").Sum(item => item.Total)
                : cartItems.Where(c => c.ServiceType != "Lẻ").Sum(item => item.Total);
            ViewBag.totalAmount = totalAmount;

            // Validate address
            if (string.IsNullOrWhiteSpace(address))
            {
                ModelState.AddModelError("address", "Địa chỉ là bắt buộc.");
                ViewBag.Address = address;
                ViewBag.Notes = notes;
                ViewBag.PaymentMethodId = paymentMethodId;
                ViewBag.ScheduledDate1 = scheduledDate1;
                ViewBag.ScheduledTime1 = scheduledTime1;
                ViewBag.ScheduledDate2 = scheduledDate2;
                ViewBag.ScheduledTime2 = scheduledTime2;
                return RedirectToAction(checkoutType == "Carts" ? "CheckOutCart" : "CheckOutContract");
            }

            // Validate schedule based on checkout type
            bool requiresSchedule = checkoutType == "Carts" || (checkoutType == "Contracts" && scheduleOption == "manual");
            if (requiresSchedule)
            {
                if (string.IsNullOrWhiteSpace(scheduledDate1) || string.IsNullOrWhiteSpace(scheduledTime1))
                {
                    ModelState.AddModelError("scheduledDate1", "Ngày và giờ chăm sóc là bắt buộc.");
                    ViewBag.Address = address;
                    ViewBag.Notes = notes;
                    ViewBag.PaymentMethodId = paymentMethodId;
                    ViewBag.ScheduledDate1 = scheduledDate1;
                    ViewBag.ScheduledTime1 = scheduledTime1;
                    ViewBag.ScheduledDate2 = scheduledDate2;
                    ViewBag.ScheduledTime2 = scheduledTime2;
                    return RedirectToAction(checkoutType == "Carts" ? "CheckOutCart" : "CheckOutContract");
                }
                if (checkoutType == "Contracts" && scheduleOption == "manual" && (string.IsNullOrWhiteSpace(scheduledDate2) || string.IsNullOrWhiteSpace(scheduledTime2)))
                {
                    ModelState.AddModelError("scheduledDate2", "Ngày và giờ chăm sóc thứ hai là bắt buộc khi tự chọn lịch.");
                    ViewBag.Address = address;
                    ViewBag.Notes = notes;
                    ViewBag.PaymentMethodId = paymentMethodId;
                    ViewBag.ScheduledDate1 = scheduledDate1;
                    ViewBag.ScheduledTime1 = scheduledTime1;
                    ViewBag.ScheduledDate2 = scheduledDate2;
                    ViewBag.ScheduledTime2 = scheduledTime2;
                    return RedirectToAction("CheckOutContract");
                }
            }

            DateTime startDate1 = requiresSchedule ? DateTime.Parse(scheduledDate1) : DateTime.Now;
            TimeOnly scheduledTimeValue1 = requiresSchedule ? TimeOnly.Parse(scheduledTime1) : TimeOnly.FromDateTime(DateTime.Now);
            DateTime startDate2 = !string.IsNullOrWhiteSpace(scheduledDate2) ? DateTime.Parse(scheduledDate2) : DateTime.Now;
            TimeOnly scheduledTimeValue2 = !string.IsNullOrWhiteSpace(scheduledTime2) ? TimeOnly.Parse(scheduledTime2) : TimeOnly.FromDateTime(DateTime.Now);

            var paymentMethod = await _context.PaymentMethods
                .FirstOrDefaultAsync(pm => pm.PaymentMethodId == paymentMethodId);

            int? orderIdForRedirect = null;
            int? contractIdForRedirect = null;
            string contractCode = null;

            // Process individual services (Lẻ) only if checkoutType is Carts
            if (checkoutType == "Carts")
            {
                var leItems = cartItems.Where(c => c.ServiceType == "Lẻ").ToList();
                if (leItems.Any())
                {
                    var order = new Order
                    {
                        CustomerId = customerId.Value,
                        OrderDate = DateTime.Now,
                        TotalPrice = leItems.Sum(i => i.Total),
                        PaymentMethodId = paymentMethodId,
                        PaymentDate = null,
                        PaymentStatus = "Chưa thanh toán",
                        Status = "Chờ xử lý"
                    };

                    _context.Orders.Add(order);
                    await _context.SaveChangesAsync();
                    orderIdForRedirect = order.OrderId;

                    foreach (var item in leItems)
                    {
                        var servicePrice = await _context.ServicePrices
                            .FirstOrDefaultAsync(p => p.ServiceId == item.ServiceId
                                                   && p.ServiceType == item.ServiceType
                                                   && p.TreeSize == item.TreeSize);

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

                        var careSchedule = new CareSchedule
                        {
                            ContractId = null,
                            OrderId = order.OrderId,
                            StaffId = null,
                            ScheduledDate = DateOnly.FromDateTime(startDate1),
                            ScheduledTime = scheduledTimeValue1,
                            ActualDate = null,
                            Duration = 2.0m,
                            Status = "Chưa thực hiện"
                        };
                        _context.CareSchedules.Add(careSchedule);
                    }
                }
            }

            // Process contracts (Tháng) only if checkoutType is Contracts
            if (checkoutType == "Contracts")
            {
                var thangItems = cartItems.Where(c => c.ServiceType != "Lẻ").ToList();
                if (thangItems.Any())
                {
                    var today = DateTime.Today;
                    var contractsTodayCount = await _context.Contracts
                        .Where(c => c.CreatedDate.HasValue && c.CreatedDate.Value.Date == today)
                        .CountAsync();

                    contractCode = $"HD-{today:yyyyMMdd}-{contractsTodayCount + 1:D3}";

                    var contract = new Models.Contract
                    {
                        CustomerId = customerId.Value,
                        ContractCode = contractCode,
                        CreatedDate = DateTime.Now,
                        TotalAmount = thangItems.Sum(i => i.Total), // Sử dụng TotalAmount theo model Contract
                        DurationUnit = "Tháng",
                        Duration = int.Parse(thangItems.First().ServiceType.Split(' ')[0]),
                        StartDate = DateOnly.FromDateTime(scheduleOption == "manual" ? startDate1 : DateTime.Now),
                        EndDate = DateOnly.FromDateTime((scheduleOption == "manual" ? startDate1 : DateTime.Now).AddMonths(int.Parse(thangItems.First().ServiceType.Split(' ')[0]))),
                        PaymentMethodId = paymentMethodId,
                        PaymentStatus = "Chưa thanh toán",
                        Status = "Chờ xử lý"
                    };

                    _context.Contracts.Add(contract);
                    await _context.SaveChangesAsync();
                    contractIdForRedirect = contract.ContractId;

                    foreach (var item in thangItems)
                    {
                        var servicePrice = await _context.ServicePrices
                            .FirstOrDefaultAsync(p => p.ServiceId == item.ServiceId
                                                   && p.ServiceType == item.ServiceType
                                                   && p.TreeSize == item.TreeSize
                                                   && p.OfficeSize == item.OfficeSize);

                        var contractDetail = new ContractDetail
                        {
                            ContractId = contract.ContractId,
                            ServiceId = item.ServiceId,
                            Address = address,
                            PriceId = servicePrice.PriceId,
                            TotalAmount = item.Total,
                            Notes = notes,
                            Status = "Đang chờ xác nhận"
                        };
                        _context.ContractDetails.Add(contractDetail);

                        int numberOfSchedules;
                        switch (item.ServiceType)
                        {
                            case "3 tháng": numberOfSchedules = 24; break; // 2 lần/tuần trong 3 tháng
                            case "6 tháng": numberOfSchedules = 48; break; // 2 lần/tuần trong 6 tháng
                            case "12 tháng": numberOfSchedules = 96; break; // 2 lần/tuần trong 12 tháng
                            default: numberOfSchedules = 0; break;
                        }

                        if (scheduleOption == "manual")
                        {
                            for (int i = 0; i < numberOfSchedules; i++)
                            {
                                DateTime baseDate = (i % 2 == 0) ? startDate1 : startDate2;
                                TimeOnly baseTime = (i % 2 == 0) ? scheduledTimeValue1 : scheduledTimeValue2;
                                var careSchedule = new CareSchedule
                                {
                                    ContractId = contract.ContractId,
                                    OrderId = null,
                                    StaffId = null,
                                    ScheduledDate = DateOnly.FromDateTime(baseDate.AddDays((i / 2) * 7)),
                                    ScheduledTime = baseTime,
                                    ActualDate = null,
                                    Duration = 2.0m,
                                    Status = "Chưa thực hiện"
                                };
                                _context.CareSchedules.Add(careSchedule);
                            }
                        }
                        else // Tự động tạo lịch ngẫu nhiên
                        {
                            var random = new Random();
                            var scheduledDates = new HashSet<DateOnly>();
                            while (scheduledDates.Count < numberOfSchedules)
                            {
                                var daysToAdd = random.Next(1, 90);
                                scheduledDates.Add(DateOnly.FromDateTime(DateTime.Now.AddDays(daysToAdd)));
                            }

                            foreach (var date in scheduledDates)
                            {
                                var careSchedule = new CareSchedule
                                {
                                    ContractId = contract.ContractId,
                                    OrderId = null,
                                    StaffId = null,
                                    ScheduledDate = date,
                                    ScheduledTime = TimeOnly.FromDateTime(DateTime.Now),
                                    ActualDate = null,
                                    Duration = 2.0m,
                                    Status = "Chưa thực hiện"
                                };
                                _context.CareSchedules.Add(careSchedule);
                            }
                        }
                    }
                }
            }

            await _context.SaveChangesAsync();
            var itemsToRemove = checkoutType == "Carts" ? cartItems.Where(c => c.ServiceType == "Lẻ").ToList() : cartItems.Where(c => c.ServiceType != "Lẻ").ToList();
            _context.Carts.RemoveRange(itemsToRemove);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Success), new { orderId = orderIdForRedirect, contractId = contractIdForRedirect, contractCode, totalAmount, address, paymentDate = DateTime.Now, paymentStatus = "Chưa thanh toán" });
        }

        // Remove item from cart
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

        // Success page after order
        public IActionResult Success(int? contractId, int? orderId, string contractCode, decimal totalAmount, string address, DateTime paymentDate, string paymentStatus)
        {
            var categories = _context.ServiceCategories
               .Include(c => c.Services)
               .ToListAsync();
            ViewData["ServiceCategories"] = categories;

            ViewBag.OrderId = orderId;
            ViewBag.ContractId = contractId;
            ViewBag.ContractCode = contractCode;
            ViewBag.TotalAmount = totalAmount;
            ViewBag.Address = address;
            ViewBag.PaymentDate = paymentDate.ToString("dd/MM/yyyy");
            ViewBag.PaymentStatus = paymentStatus;

            return View();
        }
    }
}
