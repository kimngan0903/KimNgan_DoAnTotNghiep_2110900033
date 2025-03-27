using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using OfficePlantCare.Areas.AdminQL.Models;
using OfficePlantCare.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OfficePlantCare.Areas.AdminQL.Controllers
{
    [Area("AdminQL")]
    public class RevenueReportController : Controller
    {
        private readonly OfficePlantCareContext _context;

        public RevenueReportController(OfficePlantCareContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Revenuereport(DateTime? startDate, DateTime? endDate)
        {
            var roleId = HttpContext.Session.GetInt32("RoleId");
            if (roleId != 1) // Chỉ Admin mới được truy cập
                return RedirectToAction("Index", "Login", new { area = "AdminQL" });

            // Nếu không có ngày bắt đầu hoặc ngày kết thúc, mặc định lấy 7 ngày gần nhất
            if (!startDate.HasValue || !endDate.HasValue)
            {
                startDate = DateTime.Now.AddDays(-7);
                endDate = DateTime.Now;
            }
            else
            {
                // Đảm bảo startDate <= endDate
                if (startDate > endDate)
                {
                    var temp = startDate;
                    startDate = endDate;
                    endDate = temp;
                }
            }

            // Lưu startDate và endDate vào ViewBag để hiển thị lại trên form
            ViewBag.StartDate = startDate.Value;
            ViewBag.EndDate = endDate.Value;

            // Tính tổng doanh thu từ Orders, ServiceRequests, và Contracts trong khoảng thời gian
            var orderRevenue = _context.OrderDetails
                .Join(_context.Orders,
                    od => od.OrderId,
                    o => o.OrderId,
                    (od, o) => new { OrderDetail = od, Order = o })
                .Where(joined => joined.Order.PaymentStatus == "Đã thanh toán" && joined.Order.Status == "Đã hoàn thành"
                    && joined.Order.OrderDate.HasValue && joined.Order.OrderDate.Value >= startDate && joined.Order.OrderDate.Value <= endDate)
                .Sum(joined => joined.OrderDetail.TotalAmount ?? 0M);

            var serviceRequestRevenue = _context.ServiceRequests
                .Where(sr => sr.Status == "Đã xử lý"
                    && sr.RequestDate.HasValue && sr.RequestDate.Value >= startDate && sr.RequestDate.Value <= endDate)
                .Sum(sr => sr.TotalAmount) ?? 0M;

            var contractRevenue = _context.Contracts
                .Where(c => c.Status == "Đang hiệu lực"
                    && c.StartDate.HasValue && c.StartDate.Value >= startDate && c.StartDate.Value <= endDate)
                .Sum(c => c.TotalAmount) ?? 0M;

            var totalRevenue = orderRevenue + serviceRequestRevenue + contractRevenue;

            // Doanh thu theo ngày
            var dailyRevenue = new List<DailyRevenue>();
            var dailyRevenueDetails = new List<DailyRevenueDetail>();

            // Từ Orders
            var dailyOrderRevenue = _context.Orders
                .Where(o => o.OrderDate.HasValue && o.OrderDate.Value >= startDate && o.OrderDate.Value <= endDate
                    && o.PaymentStatus == "Đã thanh toán" && o.Status == "Đã hoàn thành")
                .Join(_context.OrderDetails,
                    o => o.OrderId,
                    od => od.OrderId,
                    (o, od) => new { Order = o, OrderDetail = od })
                .GroupBy(joined => joined.Order.OrderDate.Value.Date)
                .Select(g => new
                {
                    Date = g.Key,
                    Revenue = g.Sum(x => x.OrderDetail.TotalAmount ?? 0M)
                })
                .AsEnumerable()
                .Select(g => new DailyRevenue
                {
                    Day = $"Ngày {g.Date:dd/MM/yyyy}",
                    OriginalDate = g.Date,
                    Revenue = g.Revenue
                })
                .ToList();

            var dailyOrderDetails = _context.Orders
                .Where(o => o.OrderDate.HasValue && o.OrderDate.Value >= startDate && o.OrderDate.Value <= endDate
                    && o.PaymentStatus == "Đã thanh toán" && o.Status == "Đã hoàn thành")
                .Join(_context.OrderDetails,
                    o => o.OrderId,
                    od => od.OrderId,
                    (o, od) => new { Order = o, OrderDetail = od })
                .Join(_context.Services,
                    joined => joined.OrderDetail.ServiceId,
                    s => s.ServiceId,
                    (joined, s) => new { joined.Order, joined.OrderDetail, Service = s })
                .GroupBy(joined => joined.Order.OrderDate.Value.Date)
                .Select(g => new
                {
                    Date = g.Key,
                    Revenue = g.Sum(x => x.OrderDetail.TotalAmount ?? 0M),
                    ServiceDetails = g.GroupBy(x => x.Service.ServiceName)
                                     .Select(sg => new ServiceUsageDetail
                                     {
                                         ServiceName = sg.Key,
                                         Quantity = sg.Count(),
                                         Revenue = sg.Sum(x => x.OrderDetail.TotalAmount ?? 0M)
                                     })
                                     .ToList()
                })
                .AsEnumerable()
                .Select(g => new DailyRevenueDetail
                {
                    Day = $"Ngày {g.Date:dd/MM/yyyy}",
                    OriginalDate = g.Date,
                    OrderRevenue = g.Revenue,
                    ServiceRequestRevenue = 0M,
                    ContractRevenue = 0M,
                    ServiceUsageDetails = g.ServiceDetails
                })
                .ToList();

            dailyRevenue.AddRange(dailyOrderRevenue);
            dailyRevenueDetails.AddRange(dailyOrderDetails);

            // Từ ServiceRequests
            var dailyServiceRequestRevenue = _context.ServiceRequests
                .Where(sr => sr.RequestDate.HasValue && sr.RequestDate.Value >= startDate && sr.RequestDate.Value <= endDate
                    && sr.Status == "Đã xử lý")
                .GroupBy(sr => sr.RequestDate.Value.Date)
                .Select(g => new
                {
                    Date = g.Key,
                    Revenue = g.Sum(x => x.TotalAmount) ?? 0M
                })
                .AsEnumerable()
                .Select(g => new DailyRevenue
                {
                    Day = $"Ngày {g.Date:dd/MM/yyyy}",
                    OriginalDate = g.Date,
                    Revenue = g.Revenue
                })
                .ToList();

            var dailyServiceRequestDetails = _context.ServiceRequests
                .Where(sr => sr.RequestDate.HasValue && sr.RequestDate.Value >= startDate && sr.RequestDate.Value <= endDate
                    && sr.Status == "Đã xử lý")
                .Join(_context.Services,
                    sr => sr.ServiceId,
                    s => s.ServiceId,
                    (sr, s) => new { ServiceRequest = sr, Service = s })
                .GroupBy(joined => joined.ServiceRequest.RequestDate.Value.Date)
                .Select(g => new
                {
                    Date = g.Key,
                    Revenue = g.Sum(x => x.ServiceRequest.TotalAmount) ?? 0M,
                    ServiceDetails = g.GroupBy(x => x.Service.ServiceName)
                                     .Select(sg => new ServiceUsageDetail
                                     {
                                         ServiceName = sg.Key,
                                         Quantity = sg.Count(),
                                         Revenue = sg.Sum(x => x.ServiceRequest.TotalAmount ?? 0M)
                                     })
                                     .ToList()
                })
                .AsEnumerable()
                .Select(g => new DailyRevenueDetail
                {
                    Day = $"Ngày {g.Date:dd/MM/yyyy}",
                    OriginalDate = g.Date,
                    OrderRevenue = 0M,
                    ServiceRequestRevenue = g.Revenue,
                    ContractRevenue = 0M,
                    ServiceUsageDetails = g.ServiceDetails
                })
                .ToList();

            dailyRevenue.AddRange(dailyServiceRequestRevenue);
            dailyRevenueDetails.AddRange(dailyServiceRequestDetails);

            // Từ Contracts
            var dailyContractRevenue = _context.Contracts
                .Where(c => c.StartDate.HasValue && c.StartDate.Value >= startDate && c.StartDate.Value <= endDate
                    && c.Status == "Đang hiệu lực")
                .GroupBy(c => c.StartDate.Value.Date)
                .Select(g => new
                {
                    Date = g.Key,
                    Revenue = g.Sum(x => x.TotalAmount) ?? 0M
                })
                .AsEnumerable()
                .Select(g => new DailyRevenue
                {
                    Day = $"Ngày {g.Date:dd/MM/yyyy}",
                    OriginalDate = g.Date,
                    Revenue = g.Revenue
                })
                .ToList();

            var dailyContractDetails = _context.Contracts
                .Where(c => c.StartDate.HasValue && c.StartDate.Value >= startDate && c.StartDate.Value <= endDate
                    && c.Status == "Đang hiệu lực")
                .GroupBy(c => c.StartDate.Value.Date)
                .Select(g => new
                {
                    Date = g.Key,
                    Revenue = g.Sum(x => x.TotalAmount) ?? 0M,
                    ServiceDetails = new List<ServiceUsageDetail> { new ServiceUsageDetail { ServiceName = "Contract", Quantity = g.Count(), Revenue = g.Sum(x => x.TotalAmount ?? 0M) } }
                })
                .AsEnumerable()
                .Select(g => new DailyRevenueDetail
                {
                    Day = $"Ngày {g.Date:dd/MM/yyyy}",
                    OriginalDate = g.Date,
                    OrderRevenue = 0M,
                    ServiceRequestRevenue = 0M,
                    ContractRevenue = g.Revenue,
                    ServiceUsageDetails = g.ServiceDetails
                })
                .ToList();

            dailyRevenue.AddRange(dailyContractRevenue);
            dailyRevenueDetails.AddRange(dailyContractDetails);

            // Gộp và tổng hợp doanh thu theo ngày
            var groupedDailyRevenue = dailyRevenue
                .GroupBy(dr => dr.Day)
                .Select(g => new DailyRevenue
                {
                    Day = g.Key,
                    OriginalDate = g.First().OriginalDate,
                    Revenue = g.Sum(x => x.Revenue)
                })
                .OrderBy(g => g.OriginalDate)
                .ToList();

            var groupedDailyRevenueDetails = dailyRevenueDetails
                .GroupBy(dr => dr.Day)
                .Select(g => new DailyRevenueDetail
                {
                    Day = g.Key,
                    OriginalDate = g.First().OriginalDate,
                    OrderRevenue = g.Sum(x => x.OrderRevenue),
                    ServiceRequestRevenue = g.Sum(x => x.ServiceRequestRevenue),
                    ContractRevenue = g.Sum(x => x.ContractRevenue),
                    ServiceUsageDetails = g.SelectMany(x => x.ServiceUsageDetails)
                                          .GroupBy(s => s.ServiceName)
                                          .Select(sg => new ServiceUsageDetail
                                          {
                                              ServiceName = sg.Key,
                                              Quantity = sg.Sum(x => x.Quantity),
                                              Revenue = sg.Sum(x => x.Revenue)
                                          })
                                          .ToList()
                })
                .OrderBy(g => g.OriginalDate)
                .ToList();

            // Doanh thu theo tháng
            var monthlyRevenue = new List<MonthlyRevenue>();
            var monthlyRevenueDetails = new List<MonthlyRevenueDetail>();

            // Từ Orders
            var monthlyOrderRevenue = _context.Orders
                .Where(o => o.OrderDate.HasValue && o.OrderDate.Value >= startDate && o.OrderDate.Value <= endDate
                    && o.PaymentStatus == "Đã thanh toán" && o.Status == "Đã hoàn thành")
                .Join(_context.OrderDetails,
                    o => o.OrderId,
                    od => od.OrderId,
                    (o, od) => new { Order = o, OrderDetail = od })
                .GroupBy(joined => new { Month = joined.Order.OrderDate.Value.Month, Year = joined.Order.OrderDate.Value.Year })
                .Select(g => new
                {
                    Date = new DateTime(g.Key.Year, g.Key.Month, 1),
                    Month = g.Key.Month,
                    Year = g.Key.Year,
                    Revenue = g.Sum(x => x.OrderDetail.TotalAmount ?? 0M)
                })
                .AsEnumerable()
                .Select(g => new MonthlyRevenue
                {
                    Month = $"Tháng {g.Month}/{g.Year}",
                    OriginalDate = g.Date,
                    Revenue = g.Revenue
                })
                .ToList();

            var monthlyOrderDetails = _context.Orders
                .Where(o => o.OrderDate.HasValue && o.OrderDate.Value >= startDate && o.OrderDate.Value <= endDate
                    && o.PaymentStatus == "Đã thanh toán" && o.Status == "Đã hoàn thành")
                .Join(_context.OrderDetails,
                    o => o.OrderId,
                    od => od.OrderId,
                    (o, od) => new { Order = o, OrderDetail = od })
                .Join(_context.Services,
                    joined => joined.OrderDetail.ServiceId,
                    s => s.ServiceId,
                    (joined, s) => new { joined.Order, joined.OrderDetail, Service = s })
                .GroupBy(joined => new { Month = joined.Order.OrderDate.Value.Month, Year = joined.Order.OrderDate.Value.Year })
                .Select(g => new
                {
                    Date = new DateTime(g.Key.Year, g.Key.Month, 1),
                    Month = g.Key.Month,
                    Year = g.Key.Year,
                    Revenue = g.Sum(x => x.OrderDetail.TotalAmount ?? 0M),
                    ServiceDetails = g.GroupBy(x => x.Service.ServiceName)
                                     .Select(sg => new ServiceUsageDetail
                                     {
                                         ServiceName = sg.Key,
                                         Quantity = sg.Count(),
                                         Revenue = sg.Sum(x => x.OrderDetail.TotalAmount ?? 0M)
                                     })
                                     .ToList()
                })
                .AsEnumerable()
                .Select(g => new MonthlyRevenueDetail
                {
                    Month = $"Tháng {g.Month}/{g.Year}",
                    OriginalDate = g.Date,
                    OrderRevenue = g.Revenue,
                    ServiceRequestRevenue = 0M,
                    ContractRevenue = 0M,
                    ServiceUsageDetails = g.ServiceDetails
                })
                .ToList();

            monthlyRevenue.AddRange(monthlyOrderRevenue);
            monthlyRevenueDetails.AddRange(monthlyOrderDetails);

            // Từ ServiceRequests
            var monthlyServiceRequestRevenue = _context.ServiceRequests
                .Where(sr => sr.RequestDate.HasValue && sr.RequestDate.Value >= startDate && sr.RequestDate.Value <= endDate
                    && sr.Status == "Đã xử lý")
                .GroupBy(sr => new { Month = sr.RequestDate.Value.Month, Year = sr.RequestDate.Value.Year })
                .Select(g => new
                {
                    Date = new DateTime(g.Key.Year, g.Key.Month, 1),
                    Month = g.Key.Month,
                    Year = g.Key.Year,
                    Revenue = g.Sum(x => x.TotalAmount) ?? 0M
                })
                .AsEnumerable()
                .Select(g => new MonthlyRevenue
                {
                    Month = $"Tháng {g.Month}/{g.Year}",
                    OriginalDate = g.Date,
                    Revenue = g.Revenue
                })
                .ToList();

            var monthlyServiceRequestDetails = _context.ServiceRequests
                .Where(sr => sr.RequestDate.HasValue && sr.RequestDate.Value >= startDate && sr.RequestDate.Value <= endDate
                    && sr.Status == "Đã xử lý")
                .Join(_context.Services,
                    sr => sr.ServiceId,
                    s => s.ServiceId,
                    (sr, s) => new { ServiceRequest = sr, Service = s })
                .GroupBy(joined => new { Month = joined.ServiceRequest.RequestDate.Value.Month, Year = joined.ServiceRequest.RequestDate.Value.Year })
                .Select(g => new
                {
                    Date = new DateTime(g.Key.Year, g.Key.Month, 1),
                    Month = g.Key.Month,
                    Year = g.Key.Year,
                    Revenue = g.Sum(x => x.ServiceRequest.TotalAmount) ?? 0M,
                    ServiceDetails = g.GroupBy(x => x.Service.ServiceName)
                                     .Select(sg => new ServiceUsageDetail
                                     {
                                         ServiceName = sg.Key,
                                         Quantity = sg.Count(),
                                         Revenue = sg.Sum(x => x.ServiceRequest.TotalAmount ?? 0M)
                                     })
                                     .ToList()
                })
                .AsEnumerable()
                .Select(g => new MonthlyRevenueDetail
                {
                    Month = $"Tháng {g.Month}/{g.Year}",
                    OriginalDate = g.Date,
                    OrderRevenue = 0M,
                    ServiceRequestRevenue = g.Revenue,
                    ContractRevenue = 0M,
                    ServiceUsageDetails = g.ServiceDetails
                })
                .ToList();

            monthlyRevenue.AddRange(monthlyServiceRequestRevenue);
            monthlyRevenueDetails.AddRange(monthlyServiceRequestDetails);

            // Từ Contracts
            var monthlyContractRevenue = _context.Contracts
                .Where(c => c.StartDate.HasValue && c.StartDate.Value >= startDate && c.StartDate.Value <= endDate
                    && c.Status == "Đang hiệu lực")
                .GroupBy(c => new { Month = c.StartDate.Value.Month, Year = c.StartDate.Value.Year })
                .Select(g => new
                {
                    Date = new DateTime(g.Key.Year, g.Key.Month, 1),
                    Month = g.Key.Month,
                    Year = g.Key.Year,
                    Revenue = g.Sum(x => x.TotalAmount) ?? 0M
                })
                .AsEnumerable()
                .Select(g => new MonthlyRevenue
                {
                    Month = $"Tháng {g.Month}/{g.Year}",
                    OriginalDate = g.Date,
                    Revenue = g.Revenue
                })
                .ToList();

            var monthlyContractDetails = _context.Contracts
                .Where(c => c.StartDate.HasValue && c.StartDate.Value >= startDate && c.StartDate.Value <= endDate
                    && c.Status == "Đang hiệu lực")
                .GroupBy(c => new { Month = c.StartDate.Value.Month, Year = c.StartDate.Value.Year })
                .Select(g => new
                {
                    Date = new DateTime(g.Key.Year, g.Key.Month, 1),
                    Month = g.Key.Month,
                    Year = g.Key.Year,
                    Revenue = g.Sum(x => x.TotalAmount) ?? 0M,
                    ServiceDetails = new List<ServiceUsageDetail> { new ServiceUsageDetail { ServiceName = "Contract", Quantity = g.Count(), Revenue = g.Sum(x => x.TotalAmount ?? 0M) } }
                })
                .AsEnumerable()
                .Select(g => new MonthlyRevenueDetail
                {
                    Month = $"Tháng {g.Month}/{g.Year}",
                    OriginalDate = g.Date,
                    OrderRevenue = 0M,
                    ServiceRequestRevenue = 0M,
                    ContractRevenue = g.Revenue,
                    ServiceUsageDetails = g.ServiceDetails
                })
                .ToList();

            monthlyRevenue.AddRange(monthlyContractRevenue);
            monthlyRevenueDetails.AddRange(monthlyContractDetails);

            // Gộp và tổng hợp doanh thu theo tháng
            var groupedMonthlyRevenue = monthlyRevenue
                .GroupBy(mr => mr.Month)
                .Select(g => new MonthlyRevenue
                {
                    Month = g.Key,
                    OriginalDate = g.First().OriginalDate,
                    Revenue = g.Sum(x => x.Revenue)
                })
                .OrderBy(g => g.OriginalDate)
                .ToList();

            var groupedMonthlyRevenueDetails = monthlyRevenueDetails
                .GroupBy(mr => mr.Month)
                .Select(g => new MonthlyRevenueDetail
                {
                    Month = g.Key,
                    OriginalDate = g.First().OriginalDate,
                    OrderRevenue = g.Sum(x => x.OrderRevenue),
                    ServiceRequestRevenue = g.Sum(x => x.ServiceRequestRevenue),
                    ContractRevenue = g.Sum(x => x.ContractRevenue),
                    ServiceUsageDetails = g.SelectMany(x => x.ServiceUsageDetails)
                                          .GroupBy(s => s.ServiceName)
                                          .Select(sg => new ServiceUsageDetail
                                          {
                                              ServiceName = sg.Key,
                                              Quantity = sg.Sum(x => x.Quantity),
                                              Revenue = sg.Sum(x => x.Revenue)
                                          })
                                          .ToList()
                })
                .OrderBy(g => g.OriginalDate)
                .ToList();

            // Doanh thu theo năm
            var yearlyRevenue = new List<YearlyRevenue>();
            var yearlyRevenueDetails = new List<YearlyRevenueDetail>();

            // Từ Orders
            var yearlyOrderRevenue = _context.Orders
                .Where(o => o.OrderDate.HasValue && o.OrderDate.Value >= startDate && o.OrderDate.Value <= endDate
                    && o.Status == "Đã hoàn thành")
                .Join(_context.OrderDetails,
                    o => o.OrderId,
                    od => od.OrderId,
                    (o, od) => new { Order = o, OrderDetail = od })
                .GroupBy(joined => joined.Order.OrderDate.Value.Year)
                .Select(g => new
                {
                    Year = g.Key,
                    Revenue = g.Sum(x => x.OrderDetail.TotalAmount ?? 0M)
                })
                .AsEnumerable()
                .Select(g => new YearlyRevenue
                {
                    Year = $"Năm {g.Year}",
                    OriginalYear = g.Year,
                    Revenue = g.Revenue
                })
                .ToList();

            var yearlyOrderDetails = _context.Orders
                .Where(o => o.OrderDate.HasValue && o.OrderDate.Value >= startDate && o.OrderDate.Value <= endDate
                    && o.Status == "Đã hoàn thành")
                .Join(_context.OrderDetails,
                    o => o.OrderId,
                    od => od.OrderId,
                    (o, od) => new { Order = o, OrderDetail = od })
                .Join(_context.Services,
                    joined => joined.OrderDetail.ServiceId,
                    s => s.ServiceId,
                    (joined, s) => new { joined.Order, joined.OrderDetail, Service = s })
                .GroupBy(joined => joined.Order.OrderDate.Value.Year)
                .Select(g => new
                {
                    Year = g.Key,
                    Revenue = g.Sum(x => x.OrderDetail.TotalAmount ?? 0M),
                    ServiceDetails = g.GroupBy(x => x.Service.ServiceName)
                                     .Select(sg => new ServiceUsageDetail
                                     {
                                         ServiceName = sg.Key,
                                         Quantity = sg.Count(),
                                         Revenue = sg.Sum(x => x.OrderDetail.TotalAmount ?? 0M)
                                     })
                                     .ToList()
                })
                .AsEnumerable()
                .Select(g => new YearlyRevenueDetail
                {
                    Year = $"Năm {g.Year}",
                    OriginalYear = g.Year,
                    OrderRevenue = g.Revenue,
                    ServiceRequestRevenue = 0M,
                    ContractRevenue = 0M,
                    ServiceUsageDetails = g.ServiceDetails
                })
                .ToList();

            yearlyRevenue.AddRange(yearlyOrderRevenue);
            yearlyRevenueDetails.AddRange(yearlyOrderDetails);

            // Từ ServiceRequests
            var yearlyServiceRequestRevenue = _context.ServiceRequests
                .Where(sr => sr.RequestDate.HasValue && sr.RequestDate.Value >= startDate && sr.RequestDate.Value <= endDate
                    && sr.Status == "Đã xử lý")
                .GroupBy(sr => sr.RequestDate.Value.Year)
                .Select(g => new
                {
                    Year = g.Key,
                    Revenue = g.Sum(x => x.TotalAmount) ?? 0M
                })
                .AsEnumerable()
                .Select(g => new YearlyRevenue
                {
                    Year = $"Năm {g.Year}",
                    OriginalYear = g.Year,
                    Revenue = g.Revenue
                })
                .ToList();

            var yearlyServiceRequestDetails = _context.ServiceRequests
                .Where(sr => sr.RequestDate.HasValue && sr.RequestDate.Value >= startDate && sr.RequestDate.Value <= endDate
                    && sr.Status == "Đã xử lý")
                .Join(_context.Services,
                    sr => sr.ServiceId,
                    s => s.ServiceId,
                    (sr, s) => new { ServiceRequest = sr, Service = s })
                .GroupBy(joined => joined.ServiceRequest.RequestDate.Value.Year)
                .Select(g => new
                {
                    Year = g.Key,
                    Revenue = g.Sum(x => x.ServiceRequest.TotalAmount) ?? 0M,
                    ServiceDetails = g.GroupBy(x => x.Service.ServiceName)
                                     .Select(sg => new ServiceUsageDetail
                                     {
                                         ServiceName = sg.Key,
                                         Quantity = sg.Count(),
                                         Revenue = sg.Sum(x => x.ServiceRequest.TotalAmount ?? 0M)
                                     })
                                     .ToList()
                })
                .AsEnumerable()
                .Select(g => new YearlyRevenueDetail
                {
                    Year = $"Năm {g.Year}",
                    OriginalYear = g.Year,
                    OrderRevenue = 0M,
                    ServiceRequestRevenue = g.Revenue,
                    ContractRevenue = 0M,
                    ServiceUsageDetails = g.ServiceDetails
                })
                .ToList();

            yearlyRevenue.AddRange(yearlyServiceRequestRevenue);
            yearlyRevenueDetails.AddRange(yearlyServiceRequestDetails);

            // Từ Contracts
            var yearlyContractRevenue = _context.Contracts
                .Where(c => c.StartDate.HasValue && c.StartDate.Value >= startDate && c.StartDate.Value <= endDate
                    && c.Status == "Đang hiệu lực")
                .GroupBy(c => c.StartDate.Value.Year)
                .Select(g => new
                {
                    Year = g.Key,
                    Revenue = g.Sum(x => x.TotalAmount) ?? 0M
                })
                .AsEnumerable()
                .Select(g => new YearlyRevenue
                {
                    Year = $"Năm {g.Year}",
                    OriginalYear = g.Year,
                    Revenue = g.Revenue
                })
                .ToList();

            var yearlyContractDetails = _context.Contracts
                .Where(c => c.StartDate.HasValue && c.StartDate.Value >= startDate && c.StartDate.Value <= endDate
                    && c.Status == "Đang hiệu lực")
                .GroupBy(c => c.StartDate.Value.Year)
                .Select(g => new
                {
                    Year = g.Key,
                    Revenue = g.Sum(x => x.TotalAmount) ?? 0M,
                    ServiceDetails = new List<ServiceUsageDetail> { new ServiceUsageDetail { ServiceName = "Contract", Quantity = g.Count(), Revenue = g.Sum(x => x.TotalAmount ?? 0M) } }
                })
                .AsEnumerable()
                .Select(g => new YearlyRevenueDetail
                {
                    Year = $"Năm {g.Year}",
                    OriginalYear = g.Year,
                    OrderRevenue = 0M,
                    ServiceRequestRevenue = 0M,
                    ContractRevenue = g.Revenue,
                    ServiceUsageDetails = g.ServiceDetails
                })
                .ToList();

            yearlyRevenue.AddRange(yearlyContractRevenue);
            yearlyRevenueDetails.AddRange(yearlyContractDetails);

            // Gộp và tổng hợp doanh thu theo năm
            var groupedYearlyRevenue = yearlyRevenue
                .GroupBy(yr => yr.Year)
                .Select(g => new YearlyRevenue
                {
                    Year = g.Key,
                    OriginalYear = g.First().OriginalYear,
                    Revenue = g.Sum(x => x.Revenue)
                })
                .OrderBy(g => g.OriginalYear)
                .ToList();

            var groupedYearlyRevenueDetails = yearlyRevenueDetails
                .GroupBy(yr => yr.Year)
                .Select(g => new YearlyRevenueDetail
                {
                    Year = g.Key,
                    OriginalYear = g.First().OriginalYear,
                    OrderRevenue = g.Sum(x => x.OrderRevenue),
                    ServiceRequestRevenue = g.Sum(x => x.ServiceRequestRevenue),
                    ContractRevenue = g.Sum(x => x.ContractRevenue),
                    ServiceUsageDetails = g.SelectMany(x => x.ServiceUsageDetails)
                                          .GroupBy(s => s.ServiceName)
                                          .Select(sg => new ServiceUsageDetail
                                          {
                                              ServiceName = sg.Key,
                                              Quantity = sg.Sum(x => x.Quantity),
                                              Revenue = sg.Sum(x => x.Revenue)
                                          })
                                          .ToList()
                })
                .OrderBy(g => g.OriginalYear)
                .ToList();

            // Tạo model
            var model = new RevenueReport
            {
                TotalCustomers = _context.Customers.Count(),
                TotalContracts = _context.Contracts.Count(),
                TotalServices = _context.Services.Count(),
                TotalRevenue = totalRevenue,
                StartDate = startDate,
                EndDate = endDate,
                DailyRevenue = groupedDailyRevenue,
                MonthlyRevenue = groupedMonthlyRevenue,
                YearlyRevenue = groupedYearlyRevenue,
                DailyRevenueDetails = groupedDailyRevenueDetails,
                MonthlyRevenueDetails = groupedMonthlyRevenueDetails,
                YearlyRevenueDetails = groupedYearlyRevenueDetails
            };

            return View(model);
        }
        [HttpGet]
        public IActionResult ExportToExcelDaily(DateTime? startDate, DateTime? endDate)
        {
            var roleId = HttpContext.Session.GetInt32("RoleId");
            if (roleId != 1) // Chỉ Admin mới được truy cập
                return RedirectToAction("Index", "Login", new { area = "" });

            // Gọi lại action Revenuereport để lấy dữ liệu
            var viewResult = Revenuereport(startDate, endDate) as ViewResult;
            var model = viewResult?.Model as RevenueReport;

            if (model == null)
                return NotFound();

            // Tạo file Excel
            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Doanh thu theo ngày");

                // Tiêu đề
                worksheet.Cells[1, 1].Value = "BÁO CÁO DOANH THU THEO NGÀY";
                worksheet.Cells[1, 1, 1, 6].Merge = true;
                worksheet.Cells[1, 1].Style.Font.Bold = true;
                worksheet.Cells[1, 1].Style.Font.Size = 16;
                worksheet.Cells[1, 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                worksheet.Cells[2, 1].Value = $"Từ ngày: {(model.StartDate.HasValue ? model.StartDate.Value.ToString("dd/MM/yyyy") : "N/A")}";
                worksheet.Cells[2, 1, 2, 3].Merge = true;
                worksheet.Cells[2, 4].Value = $"Đến ngày: {(model.EndDate.HasValue ? model.EndDate.Value.ToString("dd/MM/yyyy") : "N/A")}";
                worksheet.Cells[2, 4, 2, 6].Merge = true;

                // Tiêu đề cột
                worksheet.Cells[4, 1].Value = "Ngày";
                worksheet.Cells[4, 2].Value = "Doanh thu Order (VNĐ)";
                worksheet.Cells[4, 3].Value = "Doanh thu Service Request (VNĐ)";
                worksheet.Cells[4, 4].Value = "Doanh thu Contract (VNĐ)";
                worksheet.Cells[4, 5].Value = "Tổng doanh thu (VNĐ)";
                worksheet.Cells[4, 6].Value = "Dịch vụ được sử dụng";
                worksheet.Cells[4, 1, 4, 6].Style.Font.Bold = true;
                worksheet.Cells[4, 1, 4, 6].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                worksheet.Cells[4, 1, 4, 6].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);

                // Dữ liệu
                int row = 5;
                foreach (var item in model.DailyRevenueDetails)
                {
                    worksheet.Cells[row, 1].Value = item.Day;
                    worksheet.Cells[row, 2].Value = item.OrderRevenue;
                    worksheet.Cells[row, 3].Value = item.ServiceRequestRevenue;
                    worksheet.Cells[row, 4].Value = item.ContractRevenue;
                    worksheet.Cells[row, 5].Value = item.TotalRevenue;
                    worksheet.Cells[row, 6].Value = string.Join("; ", item.ServiceUsageDetails.Select(s => $"{s.ServiceName} (Số lượng: {s.Quantity}, Doanh thu: {s.Revenue:N0} VNĐ)"));
                    row++;
                }

                // Định dạng cột
                worksheet.Cells[5, 2, row - 1, 5].Style.Numberformat.Format = "#,##0";
                worksheet.Cells[4, 1, row - 1, 6].AutoFitColumns();

                // Xuất file
                var stream = new MemoryStream();
                package.SaveAs(stream);
                stream.Position = 0;
                string excelName = $"BaoCaoDoanhThu_Ngay_{startDate?.ToString("yyyyMMdd")}_{endDate?.ToString("yyyyMMdd")}.xlsx";
                return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", excelName);
            }
        }

        [HttpGet]
        public IActionResult ExportToExcelMonthly(DateTime? startDate, DateTime? endDate)
        {
            var roleId = HttpContext.Session.GetInt32("RoleId");
            if (roleId != 1) // Chỉ Admin mới được truy cập
                return RedirectToAction("Index", "Login", new { area = "" });

            var viewResult = Revenuereport(startDate, endDate) as ViewResult;
            var model = viewResult?.Model as RevenueReport;

            if (model == null)
                return NotFound();

            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Doanh thu theo tháng");

                worksheet.Cells[1, 1].Value = "BÁO CÁO DOANH THU THEO THÁNG";
                worksheet.Cells[1, 1, 1, 6].Merge = true;
                worksheet.Cells[1, 1].Style.Font.Bold = true;
                worksheet.Cells[1, 1].Style.Font.Size = 16;
                worksheet.Cells[1, 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                worksheet.Cells[2, 1].Value = $"Từ ngày: {(model.StartDate.HasValue ? model.StartDate.Value.ToString("dd/MM/yyyy") : "N/A")}";
                worksheet.Cells[2, 1, 2, 3].Merge = true;
                worksheet.Cells[2, 4].Value = $"Đến ngày: {(model.EndDate.HasValue ? model.EndDate.Value.ToString("dd/MM/yyyy") : "N/A")}";
                worksheet.Cells[2, 4, 2, 6].Merge = true;

                worksheet.Cells[4, 1].Value = "Tháng";
                worksheet.Cells[4, 2].Value = "Doanh thu Order (VNĐ)";
                worksheet.Cells[4, 3].Value = "Doanh thu Service Request (VNĐ)";
                worksheet.Cells[4, 4].Value = "Doanh thu Contract (VNĐ)";
                worksheet.Cells[4, 5].Value = "Tổng doanh thu (VNĐ)";
                worksheet.Cells[4, 6].Value = "Dịch vụ được sử dụng";
                worksheet.Cells[4, 1, 4, 6].Style.Font.Bold = true;
                worksheet.Cells[4, 1, 4, 6].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                worksheet.Cells[4, 1, 4, 6].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);

                int row = 5;
                foreach (var item in model.MonthlyRevenueDetails)
                {
                    worksheet.Cells[row, 1].Value = item.Month;
                    worksheet.Cells[row, 2].Value = item.OrderRevenue;
                    worksheet.Cells[row, 3].Value = item.ServiceRequestRevenue;
                    worksheet.Cells[row, 4].Value = item.ContractRevenue;
                    worksheet.Cells[row, 5].Value = item.TotalRevenue;
                    worksheet.Cells[row, 6].Value = string.Join("; ", item.ServiceUsageDetails.Select(s => $"{s.ServiceName} (Số lượng: {s.Quantity}, Doanh thu: {s.Revenue:N0} VNĐ)"));
                    row++;
                }

                worksheet.Cells[5, 2, row - 1, 5].Style.Numberformat.Format = "#,##0";
                worksheet.Cells[4, 1, row - 1, 6].AutoFitColumns();

                var stream = new MemoryStream();
                package.SaveAs(stream);
                stream.Position = 0;
                string excelName = $"BaoCaoDoanhThu_Thang_{startDate?.ToString("yyyyMMdd")}_{endDate?.ToString("yyyyMMdd")}.xlsx";
                return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", excelName);
            }
        }

        [HttpGet]
        public IActionResult ExportToExcelYearly(DateTime? startDate, DateTime? endDate)
        {
            var roleId = HttpContext.Session.GetInt32("RoleId");
            if (roleId != 1) // Chỉ Admin mới được truy cập
                return RedirectToAction("Index", "Login", new { area = "" });

            var viewResult = Revenuereport(startDate, endDate) as ViewResult;
            var model = viewResult?.Model as RevenueReport;

            if (model == null)
                return NotFound();

            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Doanh thu theo năm");

                worksheet.Cells[1, 1].Value = "BÁO CÁO DOANH THU THEO NĂM";
                worksheet.Cells[1, 1, 1, 6].Merge = true;
                worksheet.Cells[1, 1].Style.Font.Bold = true;
                worksheet.Cells[1, 1].Style.Font.Size = 16;
                worksheet.Cells[1, 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                worksheet.Cells[2, 1].Value = $"Từ ngày: {(model.StartDate.HasValue ? model.StartDate.Value.ToString("dd/MM/yyyy") : "N/A")}";
                worksheet.Cells[2, 1, 2, 3].Merge = true;
                worksheet.Cells[2, 4].Value = $"Đến ngày: {(model.EndDate.HasValue ? model.EndDate.Value.ToString("dd/MM/yyyy") : "N/A")}";
                worksheet.Cells[2, 4, 2, 6].Merge = true;

                worksheet.Cells[4, 1].Value = "Năm";
                worksheet.Cells[4, 2].Value = "Doanh thu Order (VNĐ)";
                worksheet.Cells[4, 3].Value = "Doanh thu Service Request (VNĐ)";
                worksheet.Cells[4, 4].Value = "Doanh thu Contract (VNĐ)";
                worksheet.Cells[4, 5].Value = "Tổng doanh thu (VNĐ)";
                worksheet.Cells[4, 6].Value = "Dịch vụ được sử dụng";
                worksheet.Cells[4, 1, 4, 6].Style.Font.Bold = true;
                worksheet.Cells[4, 1, 4, 6].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                worksheet.Cells[4, 1, 4, 6].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);

                int row = 5;
                foreach (var item in model.YearlyRevenueDetails)
                {
                    worksheet.Cells[row, 1].Value = item.Year;
                    worksheet.Cells[row, 2].Value = item.OrderRevenue;
                    worksheet.Cells[row, 3].Value = item.ServiceRequestRevenue;
                    worksheet.Cells[row, 4].Value = item.ContractRevenue;
                    worksheet.Cells[row, 5].Value = item.TotalRevenue;
                    worksheet.Cells[row, 6].Value = string.Join("; ", item.ServiceUsageDetails.Select(s => $"{s.ServiceName} (Số lượng: {s.Quantity}, Doanh thu: {s.Revenue:N0} VNĐ)"));
                    row++;
                }

                worksheet.Cells[5, 2, row - 1, 5].Style.Numberformat.Format = "#,##0";
                worksheet.Cells[4, 1, row - 1, 6].AutoFitColumns();

                var stream = new MemoryStream();
                package.SaveAs(stream);
                stream.Position = 0;
                string excelName = $"BaoCaoDoanhThu_Nam_{startDate?.ToString("yyyyMMdd")}_{endDate?.ToString("yyyyMMdd")}.xlsx";
                return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", excelName);
            }
        }
    }
}