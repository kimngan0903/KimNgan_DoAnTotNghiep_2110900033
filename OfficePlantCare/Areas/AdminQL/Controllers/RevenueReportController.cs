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
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
        }

        [HttpGet]
        public IActionResult Revenuereport(DateTime? startDate, DateTime? endDate)
        {
            var roleId = HttpContext.Session.GetInt32("RoleId");
            if (roleId != 1) 
                return RedirectToAction("Index", "Login", new { area = "AdminQL" });

            // Default to last 7 days if no date range is provided
            if (!startDate.HasValue || !endDate.HasValue)
            {
                startDate = DateTime.Now.AddDays(-7);
                endDate = DateTime.Now;
            }
            else
            {
                // Ensure startDate <= endDate
                if (startDate > endDate)
                {
                    var temp = startDate;
                    startDate = endDate;
                    endDate = temp;
                }
            }

            // Store startDate and endDate in ViewBag for the form
            ViewBag.StartDate = startDate.Value;
            ViewBag.EndDate = endDate.Value;

            // Determine the earliest date for monthly and yearly calculations
            var earliestOrderDate = _context.Orders
                .Where(o => o.OrderDate.HasValue)
                .Min(o => o.OrderDate) ?? DateTime.Now;

            var earliestContractDate = _context.Contracts
                .Where(c => c.CreatedDate.HasValue)
                .Min(c => c.CreatedDate) ?? DateTime.Now;

            var earliestDate = new[] { earliestOrderDate, earliestContractDate }.Min();

            // Calculate total revenue for daily (using filters)
            var orderRevenueDaily = _context.Orders
                .Where(o => o.PaymentStatus == "Đã thanh toán" && o.Status == "Đã hoàn thành"
                    && o.OrderDate.HasValue && o.OrderDate.Value >= startDate && o.OrderDate.Value <= endDate)
                .Sum(o => o.TotalPrice ?? 0M);

            var contractRevenueDaily = _context.Contracts
                .Where(c => c.PaymentStatus == "Đã thanh toán" && c.Status == "Đang hiệu lực"
                    && c.CreatedDate.HasValue && c.CreatedDate.Value >= startDate && c.CreatedDate.Value <= endDate)
                .Sum(c => c.TotalAmount ?? 0M);

            var totalRevenueDaily = orderRevenueDaily + contractRevenueDaily;

            // Daily Revenue
            var dailyRevenue = new List<DailyRevenue>();
            var dailyRevenueDetails = new List<DailyRevenueDetail>();

            // From Orders (Daily)
            var dailyOrderRevenue = _context.Orders
                .Where(o => o.OrderDate.HasValue && o.OrderDate.Value >= startDate && o.OrderDate.Value <= endDate
                    && o.PaymentStatus == "Đã thanh toán" && o.Status == "Đã hoàn thành")
                .GroupBy(o => o.OrderDate.Value.Date)
                .Select(g => new
                {
                    Date = g.Key,
                    Revenue = g.Sum(x => x.TotalPrice ?? 0M)
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
                .GroupBy(o => o.OrderDate.Value.Date)
                .Select(g => new
                {
                    Date = g.Key,
                    Revenue = g.Sum(x => x.TotalPrice ?? 0M)
                })
                .AsEnumerable()
                .Select(g => new DailyRevenueDetail
                {
                    Day = $"Ngày {g.Date:dd/MM/yyyy}",
                    OriginalDate = g.Date,
                    OrderRevenue = g.Revenue,
                    ContractRevenue = 0M
                })
                .ToList();

            dailyRevenue.AddRange(dailyOrderRevenue);
            dailyRevenueDetails.AddRange(dailyOrderDetails);

            // From Contracts (Daily)
            var dailyContractRevenue = _context.Contracts
                .Where(c => c.PaymentStatus == "Đã thanh toán" && c.Status == "Đang hiệu lực"
                    && c.CreatedDate.HasValue && c.CreatedDate.Value >= startDate && c.CreatedDate.Value <= endDate)
                .GroupBy(c => c.CreatedDate.Value.Date)
                .Select(g => new
                {
                    Date = g.Key,
                    Revenue = g.Sum(x => x.TotalAmount ?? 0M)
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
                .Where(c => c.PaymentStatus == "Đã thanh toán" && c.Status == "Đang hiệu lực"
                    && c.CreatedDate.HasValue && c.CreatedDate.Value >= startDate && c.CreatedDate.Value <= endDate)
                .GroupBy(c => c.CreatedDate.Value.Date)
                .Select(g => new
                {
                    Date = g.Key,
                    Revenue = g.Sum(x => x.TotalAmount ?? 0M)
                })
                .AsEnumerable()
                .Select(g => new DailyRevenueDetail
                {
                    Day = $"Ngày {g.Date:dd/MM/yyyy}",
                    OriginalDate = g.Date,
                    OrderRevenue = 0M,
                    ContractRevenue = g.Revenue
                })
                .ToList();

            dailyRevenue.AddRange(dailyContractRevenue);
            dailyRevenueDetails.AddRange(dailyContractDetails);

            // Group and aggregate daily revenue
            var groupedDailyRevenue = dailyRevenue
                .GroupBy(dr => dr.Day)
                .Select(g => new DailyRevenue
                {
                    Day = g.Key,
                    OriginalDate = g.First().OriginalDate,
                    Revenue = g.Sum(x => x.Revenue)
                })
                .Where(g => g.Revenue > 0)
                .OrderBy(g => g.OriginalDate)
                .ToList();

            var groupedDailyRevenueDetails = dailyRevenueDetails
                .GroupBy(dr => dr.Day)
                .Select(g => new DailyRevenueDetail
                {
                    Day = g.Key,
                    OriginalDate = g.First().OriginalDate,
                    OrderRevenue = g.Sum(x => x.OrderRevenue),
                    ContractRevenue = g.Sum(x => x.ContractRevenue)
                })
                .Where(g => g.TotalRevenue > 0)
                .OrderBy(g => g.OriginalDate)
                .ToList();

            // Monthly Revenue
            var monthlyRevenue = new List<MonthlyRevenue>();
            var monthlyRevenueDetails = new List<MonthlyRevenueDetail>();

            // From Orders (Monthly)
            var monthlyOrderRevenue = _context.Orders
                .Where(o => o.OrderDate.HasValue && o.OrderDate.Value >= earliestDate
                    && o.PaymentStatus == "Đã thanh toán" && o.Status == "Đã hoàn thành")
                .GroupBy(o => new { Month = o.OrderDate.Value.Month, Year = o.OrderDate.Value.Year })
                .Select(g => new
                {
                    Date = new DateTime(g.Key.Year, g.Key.Month, 1),
                    Month = g.Key.Month,
                    Year = g.Key.Year,
                    Revenue = g.Sum(x => x.TotalPrice ?? 0M)
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
                .Where(o => o.OrderDate.HasValue && o.OrderDate.Value >= earliestDate
                    && o.PaymentStatus == "Đã thanh toán" && o.Status == "Đã hoàn thành")
                .GroupBy(o => new { Month = o.OrderDate.Value.Month, Year = o.OrderDate.Value.Year })
                .Select(g => new
                {
                    Date = new DateTime(g.Key.Year, g.Key.Month, 1),
                    Month = g.Key.Month,
                    Year = g.Key.Year,
                    Revenue = g.Sum(x => x.TotalPrice ?? 0M)
                })
                .AsEnumerable()
                .Select(g => new MonthlyRevenueDetail
                {
                    Month = $"Tháng {g.Month}/{g.Year}",
                    OriginalDate = g.Date,
                    OrderRevenue = g.Revenue,
                    ContractRevenue = 0M
                })
                .ToList();

            monthlyRevenue.AddRange(monthlyOrderRevenue);
            monthlyRevenueDetails.AddRange(monthlyOrderDetails);

            // From Contracts (Monthly)
            var monthlyContractRevenue = _context.Contracts
                .Where(c => c.PaymentStatus == "Đã thanh toán" && c.Status == "Đang hiệu lực"
                    && c.CreatedDate.HasValue && c.CreatedDate.Value >= earliestDate)
                .GroupBy(c => new { Month = c.CreatedDate.Value.Month, Year = c.CreatedDate.Value.Year })
                .Select(g => new
                {
                    Date = new DateTime(g.Key.Year, g.Key.Month, 1),
                    Month = g.Key.Month,
                    Year = g.Key.Year,
                    Revenue = g.Sum(x => x.TotalAmount ?? 0M)
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
                .Where(c => c.PaymentStatus == "Đã thanh toán" && c.Status == "Đang hiệu lực"
                    && c.CreatedDate.HasValue && c.CreatedDate.Value >= earliestDate)
                .GroupBy(c => new { Month = c.CreatedDate.Value.Month, Year = c.CreatedDate.Value.Year })
                .Select(g => new
                {
                    Date = new DateTime(g.Key.Year, g.Key.Month, 1),
                    Month = g.Key.Month,
                    Year = g.Key.Year,
                    Revenue = g.Sum(x => x.TotalAmount ?? 0M)
                })
                .AsEnumerable()
                .Select(g => new MonthlyRevenueDetail
                {
                    Month = $"Tháng {g.Month}/{g.Year}",
                    OriginalDate = g.Date,
                    OrderRevenue = 0M,
                    ContractRevenue = g.Revenue
                })
                .ToList();

            monthlyRevenue.AddRange(monthlyContractRevenue);
            monthlyRevenueDetails.AddRange(monthlyContractDetails);

            // Group and aggregate monthly revenue
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
                    ContractRevenue = g.Sum(x => x.ContractRevenue)
                })
                .OrderBy(g => g.OriginalDate)
                .ToList();

            // Yearly Revenue
            var yearlyRevenue = new List<YearlyRevenue>();
            var yearlyRevenueDetails = new List<YearlyRevenueDetail>();

            // From Orders (Yearly)
            var yearlyOrderRevenue = _context.Orders
                .Where(o => o.OrderDate.HasValue && o.OrderDate.Value >= earliestDate
                    && o.PaymentStatus == "Đã thanh toán" && o.Status == "Đã hoàn thành")
                .GroupBy(o => o.OrderDate.Value.Year)
                .Select(g => new
                {
                    Year = g.Key,
                    Revenue = g.Sum(x => x.TotalPrice ?? 0M)
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
                .Where(o => o.OrderDate.HasValue && o.OrderDate.Value >= earliestDate
                    && o.PaymentStatus == "Đã thanh toán" && o.Status == "Đã hoàn thành")
                .GroupBy(o => o.OrderDate.Value.Year)
                .Select(g => new
                {
                    Year = g.Key,
                    Revenue = g.Sum(x => x.TotalPrice ?? 0M)
                })
                .AsEnumerable()
                .Select(g => new YearlyRevenueDetail
                {
                    Year = $"Năm {g.Year}",
                    OriginalYear = g.Year,
                    OrderRevenue = g.Revenue,
                    ContractRevenue = 0M
                })
                .ToList();

            yearlyRevenue.AddRange(yearlyOrderRevenue);
            yearlyRevenueDetails.AddRange(yearlyOrderDetails);

            // From Contracts (Yearly)
            var yearlyContractRevenue = _context.Contracts
                .Where(c => c.PaymentStatus == "Đã thanh toán" && c.Status == "Đang hiệu lực"
                    && c.CreatedDate.HasValue && c.CreatedDate.Value >= earliestDate)
                .GroupBy(c => c.CreatedDate.Value.Year)
                .Select(g => new
                {
                    Year = g.Key,
                    Revenue = g.Sum(x => x.TotalAmount ?? 0M)
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
                .Where(c => c.PaymentStatus == "Đã thanh toán" && c.Status == "Đang hiệu lực"
                    && c.CreatedDate.HasValue && c.CreatedDate.Value >= earliestDate)
                .GroupBy(c => c.CreatedDate.Value.Year)
                .Select(g => new
                {
                    Year = g.Key,
                    Revenue = g.Sum(x => x.TotalAmount ?? 0M)
                })
                .AsEnumerable()
                .Select(g => new YearlyRevenueDetail
                {
                    Year = $"Năm {g.Year}",
                    OriginalYear = g.Year,
                    OrderRevenue = 0M,
                    ContractRevenue = g.Revenue
                })
                .ToList();

            yearlyRevenue.AddRange(yearlyContractRevenue);
            yearlyRevenueDetails.AddRange(yearlyContractDetails);

            // Group and aggregate yearly revenue
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
                    ContractRevenue = g.Sum(x => x.ContractRevenue)
                })
                .OrderBy(g => g.OriginalYear)
                .ToList();

            // Create model
            var model = new RevenueReport
            {
                TotalCustomers = _context.Customers.Count(),
                TotalContracts = _context.Contracts.Count(),
                TotalServices = _context.Services.Count(),
                TotalRevenue = totalRevenueDaily,
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

        // Doanh thu theo hợp đồng
        [HttpGet]
        public IActionResult ContractRevenue(DateTime? startDate, DateTime? endDate)
        {
            var roleId = HttpContext.Session.GetInt32("RoleId");
            if (roleId != 1)
                return RedirectToAction("Index", "Login", new { area = "AdminQL" });

            if (!startDate.HasValue || !endDate.HasValue)
            {
                startDate = DateTime.Now.AddDays(-7);
                endDate = DateTime.Now;
            }
            else
            {
                if (startDate > endDate)
                {
                    var temp = startDate;
                    startDate = endDate;
                    endDate = temp;
                }
            }

            ViewBag.StartDate = startDate.Value;
            ViewBag.EndDate = endDate.Value;

            var earliestContractDate = _context.Contracts
                .Where(c => c.CreatedDate.HasValue)
                .Min(c => c.CreatedDate) ?? DateTime.Now;

            var cashTotal = _context.Contracts
                .Where(c => c.PaymentMethodId == 1 && c.PaymentStatus == "Đã thanh toán" && c.Status == "Đang hiệu lực"
                    && c.CreatedDate.HasValue && c.CreatedDate.Value >= startDate && c.CreatedDate.Value <= endDate)
                .Sum(c => c.TotalAmount ?? 0M);

            var vietQRTotal = _context.Contracts
                .Where(c => c.PaymentMethodId == 2 && c.PaymentStatus == "Đã thanh toán" && c.Status == "Đang hiệu lực"
                    && c.CreatedDate.HasValue && c.CreatedDate.Value >= startDate && c.CreatedDate.Value <= endDate)
                .Sum(c => c.TotalAmount ?? 0M);

            var contractRevenueDaily = _context.Contracts
                .Where(c => c.PaymentStatus == "Đã thanh toán" && c.Status == "Đang hiệu lực"
                    && c.CreatedDate.HasValue && c.CreatedDate.Value >= startDate && c.CreatedDate.Value <= endDate)
                .Sum(c => c.TotalAmount ?? 0M);

            var dailyRevenue = _context.Contracts
                .Where(c => c.PaymentStatus == "Đã thanh toán" && c.Status == "Đang hiệu lực"
                    && c.CreatedDate.HasValue && c.CreatedDate.Value >= startDate && c.CreatedDate.Value <= endDate)
                .GroupBy(c => c.CreatedDate.Value.Date)
                .Select(g => new DailyRevenue
                {
                    Day = $"Ngày {g.Key:dd/MM/yyyy}",
                    OriginalDate = g.Key,
                    Revenue = g.Sum(x => x.TotalAmount ?? 0M)
                })
                .ToList();

            var dailyPaymentDetails = _context.Contracts
                .Where(c => c.CreatedDate.HasValue && c.CreatedDate.Value >= startDate && c.CreatedDate.Value <= endDate
                    && c.PaymentStatus == "Đã thanh toán" && c.Status == "Đang hiệu lực")
                .GroupBy(c => c.CreatedDate.Value.Date)
                .Select(g => new
                {
                    Date = g.Key,
                    Cash = g.Where(c => c.PaymentMethodId == 1).Sum(x => x.TotalAmount ?? 0M),
                    VietQR = g.Where(c => c.PaymentMethodId == 2).Sum(x => x.TotalAmount ?? 0M)
                })
                .AsEnumerable()
                .Select(g => new DailyPaymentDetail
                {
                    Day = $"Ngày {g.Date:dd/MM/yyyy}",
                    OriginalDate = g.Date,
                    CashAmount = g.Cash,
                    VietQRAmount = g.VietQR
                })
                .ToList();

            var monthlyRevenue = _context.Contracts
                .Where(c => c.PaymentStatus == "Đã thanh toán" && c.Status == "Đang hiệu lực"
                    && c.CreatedDate.HasValue && c.CreatedDate.Value >= earliestContractDate)
                .GroupBy(c => new { Month = c.CreatedDate.Value.Month, Year = c.CreatedDate.Value.Year })
                .Select(g => new MonthlyRevenue
                {
                    Month = $"Tháng {g.Key.Month}/{g.Key.Year}",
                    OriginalDate = new DateTime(g.Key.Year, g.Key.Month, 1),
                    Revenue = g.Sum(x => x.TotalAmount ?? 0M)
                })
                .ToList();

            var monthlyPaymentDetails = _context.Contracts
                .Where(c => c.CreatedDate.HasValue && c.CreatedDate.Value >= earliestContractDate
                    && c.PaymentStatus == "Đã thanh toán" && c.Status == "Đang hiệu lực")
                .GroupBy(c => new { Month = c.CreatedDate.Value.Month, Year = c.CreatedDate.Value.Year })
                .Select(g => new
                {
                    Date = new DateTime(g.Key.Year, g.Key.Month, 1),
                    Cash = g.Where(c => c.PaymentMethodId == 1).Sum(x => x.TotalAmount ?? 0M),
                    VietQR = g.Where(c => c.PaymentMethodId == 2).Sum(x => x.TotalAmount ?? 0M)
                })
                .AsEnumerable()
                .Select(g => new MonthlyPaymentDetail
                {
                    Month = $"Tháng {g.Date.Month}/{g.Date.Year}",
                    OriginalDate = g.Date,
                    CashAmount = g.Cash,
                    VietQRAmount = g.VietQR
                })
                .ToList();

            var yearlyRevenue = _context.Contracts
                .Where(c => c.PaymentStatus == "Đã thanh toán" && c.Status == "Đang hiệu lực"
                    && c.CreatedDate.HasValue && c.CreatedDate.Value >= earliestContractDate)
                .GroupBy(c => c.CreatedDate.Value.Year)
                .Select(g => new YearlyRevenue
                {
                    Year = $"Năm {g.Key}",
                    OriginalYear = g.Key,
                    Revenue = g.Sum(x => x.TotalAmount ?? 0M)
                })
                .ToList();

            var yearlyPaymentDetails = _context.Contracts
                .Where(c => c.CreatedDate.HasValue && c.CreatedDate.Value >= earliestContractDate
                    && c.PaymentStatus == "Đã thanh toán" && c.Status == "Đang hiệu lực")
                .GroupBy(c => c.CreatedDate.Value.Year)
                .Select(g => new
                {
                    Year = g.Key,
                    Cash = g.Where(c => c.PaymentMethodId == 1).Sum(x => x.TotalAmount ?? 0M),
                    VietQR = g.Where(c => c.PaymentMethodId == 2).Sum(x => x.TotalAmount ?? 0M)
                })
                .AsEnumerable()
                .Select(g => new YearlyPaymentDetail
                {
                    Year = $"Năm {g.Year}",
                    OriginalYear = g.Year,
                    CashAmount = g.Cash,
                    VietQRAmount = g.VietQR
                })
                .ToList();

            var dailyRevenueDetails = dailyRevenue.Select(dr => new DailyRevenueDetail
            {
                Day = dr.Day,
                OriginalDate = dr.OriginalDate,
                ContractRevenue = dr.Revenue,
                OrderRevenue = 0M
            }).ToList();

            var monthlyRevenueDetails = monthlyRevenue.Select(mr => new MonthlyRevenueDetail
            {
                Month = mr.Month,
                OriginalDate = mr.OriginalDate,
                ContractRevenue = mr.Revenue,
                OrderRevenue = 0M
            }).ToList();

            var yearlyRevenueDetails = yearlyRevenue.Select(yr => new YearlyRevenueDetail
            {
                Year = yr.Year,
                OriginalYear = yr.OriginalYear,
                ContractRevenue = yr.Revenue,
                OrderRevenue = 0M
            }).ToList();

            var model = new RevenueReport
            {
                TotalContracts = _context.Contracts.Count(),
                TotalRevenue = contractRevenueDaily,
                StartDate = startDate,
                EndDate = endDate,
                DailyRevenue = dailyRevenue,
                MonthlyRevenue = monthlyRevenue,
                YearlyRevenue = yearlyRevenue,
                DailyRevenueDetails = dailyRevenueDetails,
                MonthlyRevenueDetails = monthlyRevenueDetails,
                YearlyRevenueDetails = yearlyRevenueDetails,
                DailyPaymentDetails = dailyPaymentDetails,
                MonthlyPaymentDetails = monthlyPaymentDetails,
                YearlyPaymentDetails = yearlyPaymentDetails,
                CashTotal = cashTotal,
                VietQRTotal = vietQRTotal,
                ReportType = "Contracts"
            };

            return View("RevenueDetail", model);
        }

        // Doanh thu theo đơn hàng
        [HttpGet]
        public IActionResult OrderRevenue(DateTime? startDate, DateTime? endDate)
        {
            var roleId = HttpContext.Session.GetInt32("RoleId");
            if (roleId != 1)
                return RedirectToAction("Index", "Login", new { area = "AdminQL" });

            if (!startDate.HasValue || !endDate.HasValue)
            {
                startDate = DateTime.Now.AddDays(-7);
                endDate = DateTime.Now;
            }
            else
            {
                if (startDate > endDate)
                {
                    var temp = startDate;
                    startDate = endDate;
                    endDate = temp;
                }
            }

            ViewBag.StartDate = startDate.Value;
            ViewBag.EndDate = endDate.Value;

            var earliestOrderDate = _context.Orders
                .Where(o => o.OrderDate.HasValue)
                .Min(o => o.OrderDate) ?? DateTime.Now;

            var cashTotal = _context.Orders
                .Where(o => o.PaymentMethodId == 1 && o.PaymentStatus == "Đã thanh toán" && o.Status == "Đã hoàn thành"
                    && o.OrderDate.HasValue && o.OrderDate.Value >= startDate && o.OrderDate.Value <= endDate)
                .Sum(o => o.TotalPrice ?? 0M);

            var vietQRTotal = _context.Orders
                .Where(o => o.PaymentMethodId == 2 && o.PaymentStatus == "Đã thanh toán" && o.Status == "Đã hoàn thành"
                    && o.OrderDate.HasValue && o.OrderDate.Value >= startDate && o.OrderDate.Value <= endDate)
                .Sum(o => o.TotalPrice ?? 0M);

            var orderRevenueDaily = _context.Orders
                .Where(o => o.PaymentStatus == "Đã thanh toán" && o.Status == "Đã hoàn thành"
                    && o.OrderDate.HasValue && o.OrderDate.Value >= startDate && o.OrderDate.Value <= endDate)
                .Sum(o => o.TotalPrice ?? 0M);

            var dailyRevenue = _context.Orders
                .Where(o => o.PaymentStatus == "Đã thanh toán" && o.Status == "Đã hoàn thành"
                    && o.OrderDate.HasValue && o.OrderDate.Value >= startDate && o.OrderDate.Value <= endDate)
                .GroupBy(o => o.OrderDate.Value.Date)
                .Select(g => new DailyRevenue
                {
                    Day = $"Ngày {g.Key:dd/MM/yyyy}",
                    OriginalDate = g.Key,
                    Revenue = g.Sum(x => x.TotalPrice ?? 0M)
                })
                .ToList();

            var dailyPaymentDetails = _context.Orders
                .Where(o => o.OrderDate.HasValue && o.OrderDate.Value >= startDate && o.OrderDate.Value <= endDate
                    && o.PaymentStatus == "Đã thanh toán" && o.Status == "Đã hoàn thành")
                .GroupBy(o => o.OrderDate.Value.Date)
                .Select(g => new
                {
                    Date = g.Key,
                    Cash = g.Where(o => o.PaymentMethodId == 1).Sum(x => x.TotalPrice ?? 0M),
                    VietQR = g.Where(o => o.PaymentMethodId == 2).Sum(x => x.TotalPrice ?? 0M)
                })
                .AsEnumerable()
                .Select(g => new DailyPaymentDetail
                {
                    Day = $"Ngày {g.Date:dd/MM/yyyy}",
                    OriginalDate = g.Date,
                    CashAmount = g.Cash,
                    VietQRAmount = g.VietQR
                })
                .ToList();

            var monthlyRevenue = _context.Orders
                .Where(o => o.PaymentStatus == "Đã thanh toán" && o.Status == "Đã hoàn thành"
                    && o.OrderDate.HasValue && o.OrderDate.Value >= earliestOrderDate)
                .GroupBy(o => new { Month = o.OrderDate.Value.Month, Year = o.OrderDate.Value.Year })
                .Select(g => new MonthlyRevenue
                {
                    Month = $"Tháng {g.Key.Month}/{g.Key.Year}",
                    OriginalDate = new DateTime(g.Key.Year, g.Key.Month, 1),
                    Revenue = g.Sum(x => x.TotalPrice ?? 0M)
                })
                .ToList();

            var monthlyPaymentDetails = _context.Orders
                .Where(o => o.OrderDate.HasValue && o.OrderDate.Value >= earliestOrderDate
                    && o.PaymentStatus == "Đã thanh toán" && o.Status == "Đã hoàn thành")
                .GroupBy(o => new { Month = o.OrderDate.Value.Month, Year = o.OrderDate.Value.Year })
                .Select(g => new
                {
                    Date = new DateTime(g.Key.Year, g.Key.Month, 1),
                    Cash = g.Where(o => o.PaymentMethodId == 1).Sum(x => x.TotalPrice ?? 0M),
                    VietQR = g.Where(o => o.PaymentMethodId == 2).Sum(x => x.TotalPrice ?? 0M)
                })
                .AsEnumerable()
                .Select(g => new MonthlyPaymentDetail
                {
                    Month = $"Tháng {g.Date.Month}/{g.Date.Year}",
                    OriginalDate = g.Date,
                    CashAmount = g.Cash,
                    VietQRAmount = g.VietQR
                })
                .ToList();

            var yearlyRevenue = _context.Orders
                .Where(o => o.PaymentStatus == "Đã thanh toán" && o.Status == "Đã hoàn thành"
                    && o.OrderDate.HasValue && o.OrderDate.Value >= earliestOrderDate)
                .GroupBy(o => o.OrderDate.Value.Year)
                .Select(g => new YearlyRevenue
                {
                    Year = $"Năm {g.Key}",
                    OriginalYear = g.Key,
                    Revenue = g.Sum(x => x.TotalPrice ?? 0M)
                })
                .ToList();

            var yearlyPaymentDetails = _context.Orders
                .Where(o => o.OrderDate.HasValue && o.OrderDate.Value >= earliestOrderDate
                    && o.PaymentStatus == "Đã thanh toán" && o.Status == "Đã hoàn thành")
                .GroupBy(o => o.OrderDate.Value.Year)
                .Select(g => new
                {
                    Year = g.Key,
                    Cash = g.Where(o => o.PaymentMethodId == 1).Sum(x => x.TotalPrice ?? 0M),
                    VietQR = g.Where(o => o.PaymentMethodId == 2).Sum(x => x.TotalPrice ?? 0M)
                })
                .AsEnumerable()
                .Select(g => new YearlyPaymentDetail
                {
                    Year = $"Năm {g.Year}",
                    OriginalYear = g.Year,
                    CashAmount = g.Cash,
                    VietQRAmount = g.VietQR
                })
                .ToList();

            var dailyRevenueDetails = dailyRevenue.Select(dr => new DailyRevenueDetail
            {
                Day = dr.Day,
                OriginalDate = dr.OriginalDate,
                OrderRevenue = dr.Revenue,
                ContractRevenue = 0M
            }).ToList();

            var monthlyRevenueDetails = monthlyRevenue.Select(mr => new MonthlyRevenueDetail
            {
                Month = mr.Month,
                OriginalDate = mr.OriginalDate,
                OrderRevenue = mr.Revenue,
                ContractRevenue = 0M
            }).ToList();

            var yearlyRevenueDetails = yearlyRevenue.Select(yr => new YearlyRevenueDetail
            {
                Year = yr.Year,
                OriginalYear = yr.OriginalYear,
                OrderRevenue = yr.Revenue,
                ContractRevenue = 0M
            }).ToList();

            var model = new RevenueReport
            {
                TotalRevenue = orderRevenueDaily,
                StartDate = startDate,
                EndDate = endDate,
                DailyRevenue = dailyRevenue,
                MonthlyRevenue = monthlyRevenue,
                YearlyRevenue = yearlyRevenue,
                DailyRevenueDetails = dailyRevenueDetails,
                MonthlyRevenueDetails = monthlyRevenueDetails,
                YearlyRevenueDetails = yearlyRevenueDetails,
                DailyPaymentDetails = dailyPaymentDetails,
                MonthlyPaymentDetails = monthlyPaymentDetails,
                YearlyPaymentDetails = yearlyPaymentDetails,
                CashTotal = cashTotal,
                VietQRTotal = vietQRTotal,
                ReportType = "Orders"
            };

            return View("RevenueDetail", model);
        }

        // Xuất Excel cho Doanh thu theo hợp đồng
        //[HttpGet]
        //public IActionResult ExportToExcelContract(DateTime? startDate, DateTime? endDate)
        //{
        //    var roleId = HttpContext.Session.GetInt32("RoleId");
        //    if (roleId != 1) // Chỉ Admin mới được truy cập
        //        return RedirectToAction("Index", "Login", new { area = "" });

        //    var viewResult = ContractRevenue(startDate, endDate) as ViewResult;
        //    var model = viewResult?.Model as RevenueReport;

        //    if (model == null || !model.DailyRevenueDetails.Any() || !model.DailyPaymentDetails.Any())
        //        return NotFound();

        //    // Tạo file Excel
        //    using (var package = new ExcelPackage())
        //    {
        //        var worksheet = package.Workbook.Worksheets.Add("Doanh thu theo hợp đồng");

        //        worksheet.Cells[1, 1].Value = "BÁO CÁO DOANH THU THEO HỢP ĐỒNG";
        //        worksheet.Cells[1, 1, 1, 5].Merge = true;
        //        worksheet.Cells[1, 1].Style.Font.Bold = true;
        //        worksheet.Cells[1, 1].Style.Font.Size = 16;
        //        worksheet.Cells[1, 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

        //        worksheet.Cells[2, 1].Value = $"Từ ngày: {(model.StartDate.HasValue ? model.StartDate.Value.ToString("dd/MM/yyyy") : "N/A")}";
        //        worksheet.Cells[2, 1, 2, 2].Merge = true;
        //        worksheet.Cells[2, 3].Value = $"Đến ngày: {(model.EndDate.HasValue ? model.EndDate.Value.ToString("dd/MM/yyyy") : "N/A")}";
        //        worksheet.Cells[2, 3, 2, 4].Merge = true;

        //        worksheet.Cells[4, 1].Value = "Ngày";
        //        worksheet.Cells[4, 2].Value = "Thanh toán Tiền mặt (VNĐ)";
        //        worksheet.Cells[4, 3].Value = "Thanh toán VietQR (VNĐ)";
        //        worksheet.Cells[4, 4].Value = "Doanh thu hợp đồng (VNĐ)";
        //        worksheet.Cells[4, 5].Value = "Tổng doanh thu (VNĐ)";
        //        worksheet.Cells[4, 1, 4, 5].Style.Font.Bold = true;
        //        worksheet.Cells[4, 1, 4, 5].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
        //        worksheet.Cells[4, 1, 4, 5].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);

        //        int row = 6;
        //        foreach (var item in model.DailyRevenueDetails.Join(model.DailyPaymentDetails,
        //            dr => dr.Day, dp => dp.Day,
        //            (dr, dp) => new { dr.Day, dp.CashAmount, dp.VietQRAmount, dr.ContractRevenue, dr.TotalRevenue }))
        //        {
        //            worksheet.Cells[row, 1].Value = item.Day;
        //            worksheet.Cells[row, 2].Value = item.CashAmount;
        //            worksheet.Cells[row, 3].Value = item.VietQRAmount;
        //            worksheet.Cells[row, 4].Value = item.ContractRevenue;
        //            worksheet.Cells[row, 5].Value = item.TotalRevenue;
        //            row++;
        //        }

        //        // Định dạng cột
        //        if (row > 5) // Chỉ định dạng nếu có từ 1 dòng dữ liệu trở lên
        //        {
        //            worksheet.Cells[5, 2, row - 1, 5].Style.Numberformat.Format = "#.##0";
        //            worksheet.Cells[4, 1, row - 1, 6].AutoFitColumns();
        //        }
        //        else
        //        {
        //            worksheet.Cells[4, 1, row - 1, 6].AutoFitColumns();
        //        }

        //        var stream = new MemoryStream();
        //        package.SaveAs(stream);
        //        stream.Position = 0;
        //        string excelName = $"BaoCaoDoanhThu_HopDong_{startDate?.ToString("yyyyMMdd")}_{endDate?.ToString("yyyyMMdd")}.xlsx";
        //        return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", excelName);
        //    }
        //}

        //// Xuất Excel cho Doanh thu theo đơn hàng
        //[HttpGet]
        //public IActionResult ExportToExcelOrder(DateTime? startDate, DateTime? endDate)
        //{
        //    var roleId = HttpContext.Session.GetInt32("RoleId");
        //    if (roleId != 1) // Chỉ Admin mới được truy cập
        //        return RedirectToAction("Index", "Login", new { area = "" });

        //    var viewResult = OrderRevenue(startDate, endDate) as ViewResult;
        //    var model = viewResult?.Model as RevenueReport;

        //    if (model == null || !model.DailyRevenueDetails.Any() || !model.DailyPaymentDetails.Any())
        //        return NotFound();

        //    // Tạo file Excel
        //    using (var package = new ExcelPackage())
        //    {
        //        var worksheet = package.Workbook.Worksheets.Add("Doanh thu theo đơn hàng");

        //        worksheet.Cells[1, 1].Value = "BÁO CÁO DOANH THU THEO ĐƠN HÀNG";
        //        worksheet.Cells[1, 1, 1, 5].Merge = true;
        //        worksheet.Cells[1, 1].Style.Font.Bold = true;
        //        worksheet.Cells[1, 1].Style.Font.Size = 16;
        //        worksheet.Cells[1, 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

        //        worksheet.Cells[2, 1].Value = $"Từ ngày: {(model.StartDate.HasValue ? model.StartDate.Value.ToString("dd/MM/yyyy") : "N/A")}";
        //        worksheet.Cells[2, 1, 2, 2].Merge = true;
        //        worksheet.Cells[2, 3].Value = $"Đến ngày: {(model.EndDate.HasValue ? model.EndDate.Value.ToString("dd/MM/yyyy") : "N/A")}";
        //        worksheet.Cells[2, 3, 2, 4].Merge = true;

        //        worksheet.Cells[4, 1].Value = "Ngày";
        //        worksheet.Cells[4, 2].Value = "Thanh toán Tiền mặt (VNĐ)";
        //        worksheet.Cells[4, 3].Value = "Thanh toán VietQR (VNĐ)";
        //        worksheet.Cells[4, 4].Value = "Doanh thu đơn hàng (VNĐ)";
        //        worksheet.Cells[4, 5].Value = "Tổng doanh thu (VNĐ)";
        //        worksheet.Cells[4, 1, 4, 5].Style.Font.Bold = true;
        //        worksheet.Cells[4, 1, 4, 5].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
        //        worksheet.Cells[4, 1, 4, 5].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);

        //        int row = 6;
        //        foreach (var item in model.DailyRevenueDetails.Join(model.DailyPaymentDetails,
        //            dr => dr.Day, dp => dp.Day,
        //            (dr, dp) => new { dr.Day, dp.CashAmount, dp.VietQRAmount, dr.OrderRevenue, dr.TotalRevenue }))
        //        {
        //            worksheet.Cells[row, 1].Value = item.Day;
        //            worksheet.Cells[row, 2].Value = item.CashAmount;
        //            worksheet.Cells[row, 3].Value = item.VietQRAmount;
        //            worksheet.Cells[row, 4].Value = item.OrderRevenue;
        //            worksheet.Cells[row, 5].Value = item.TotalRevenue;
        //            row++;
        //        }

        //        // Định dạng cột
        //        if (row > 5) // Chỉ định dạng nếu có từ 1 dòng dữ liệu trở lên
        //        {
        //            worksheet.Cells[5, 2, row - 1, 5].Style.Numberformat.Format = "#.##0";
        //            worksheet.Cells[4, 1, row - 1, 6].AutoFitColumns();
        //        }
        //        else
        //        {
        //            worksheet.Cells[4, 1, row - 1, 6].AutoFitColumns();
        //        }

        //        var stream = new MemoryStream();
        //        package.SaveAs(stream);
        //        stream.Position = 0;
        //        string excelName = $"BaoCaoDoanhThu_DonHang_{startDate?.ToString("yyyyMMdd")}_{endDate?.ToString("yyyyMMdd")}.xlsx";
        //        return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", excelName);
        //    }
        //}

        //[HttpGet]
        //public IActionResult ExportToExcelDaily(DateTime? startDate, DateTime? endDate)
        //{
        //    var roleId = HttpContext.Session.GetInt32("RoleId");
        //    if (roleId != 1) // Chỉ Admin mới được truy cập
        //        return RedirectToAction("Index", "Login", new { area = "" });

        //    var viewResult = Revenuereport(startDate, endDate) as ViewResult;
        //    var model = viewResult?.Model as RevenueReport;

        //    if (model == null)
        //        return NotFound();

        //    // Tạo file Excel
        //    using (var package = new ExcelPackage())
        //    {
        //        var worksheet = package.Workbook.Worksheets.Add("Doanh thu theo ngày");

        //        worksheet.Cells[1, 1].Value = "BÁO CÁO DOANH THU THEO NGÀY";
        //        worksheet.Cells[1, 1, 1, 6].Merge = true;
        //        worksheet.Cells[1, 1].Style.Font.Bold = true;
        //        worksheet.Cells[1, 1].Style.Font.Size = 16;
        //        worksheet.Cells[1, 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

        //        worksheet.Cells[2, 1].Value = $"Từ ngày: {(model.StartDate.HasValue ? model.StartDate.Value.ToString("dd/MM/yyyy") : "N/A")}";
        //        worksheet.Cells[2, 1, 2, 3].Merge = true;
        //        worksheet.Cells[2, 4].Value = $"Đến ngày: {(model.EndDate.HasValue ? model.EndDate.Value.ToString("dd/MM/yyyy") : "N/A")}";
        //        worksheet.Cells[2, 4, 2, 6].Merge = true;

        //        worksheet.Cells[4, 1].Value = "Ngày";
        //        worksheet.Cells[4, 2].Value = "Doanh thu đơn dịch vụ lẻ đã đặt (VNĐ)";
        //        worksheet.Cells[4, 3].Value = "Doanh thu hợp đồng (VNĐ)";
        //        worksheet.Cells[4, 4].Value = "Tổng doanh thu (VNĐ)";
        //        worksheet.Cells[4, 1, 4, 6].Style.Font.Bold = true;
        //        worksheet.Cells[4, 1, 4, 6].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
        //        worksheet.Cells[4, 1, 4, 6].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);

        //        // Dữ liệu
        //        int row = 5;
        //        foreach (var item in model.DailyRevenueDetails)
        //        {
        //            worksheet.Cells[row, 1].Value = item.Day;
        //            worksheet.Cells[row, 2].Value = item.OrderRevenue;
        //            worksheet.Cells[row, 3].Value = item.ContractRevenue;
        //            worksheet.Cells[row, 4].Value = item.TotalRevenue;
        //            row++;
        //        }
        //        // Định dạng cột
        //        if (row > 5) // Chỉ định dạng nếu có từ 1 dòng dữ liệu trở lên
        //        {
        //            worksheet.Cells[5, 2, row - 1, 5].Style.Numberformat.Format = "#.##0";
        //            worksheet.Cells[4, 1, row - 1, 6].AutoFitColumns();
        //        }
        //        else
        //        {
        //            worksheet.Cells[4, 1, row - 1, 6].AutoFitColumns();
        //        }

        //        var stream = new MemoryStream();
        //        package.SaveAs(stream);
        //        stream.Position = 0;
        //        string excelName = $"BaoCaoDoanhThu_Ngay_{startDate?.ToString("yyyyMMdd")}_{endDate?.ToString("yyyyMMdd")}.xlsx";
        //        return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", excelName);
        //    }
        //}

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

            // Tạo file Excel
            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Doanh thu theo tháng");

                worksheet.Cells[1, 1].Value = "BÁO CÁO DOANH THU THEO THÁNG";
                worksheet.Cells[1, 1, 1, 3].Merge = true;
                worksheet.Cells[1, 1].Style.Font.Bold = true;
                worksheet.Cells[1, 1].Style.Font.Size = 16;
                worksheet.Cells[1, 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                worksheet.Cells[2, 1].Value = $"Từ ngày: {(model.StartDate.HasValue ? model.StartDate.Value.ToString("dd/MM/yyyy") : "N/A")}";
                worksheet.Cells[2, 1, 2, 2].Merge = true;
                worksheet.Cells[2, 3].Value = $"Đến ngày: {(model.EndDate.HasValue ? model.EndDate.Value.ToString("dd/MM/yyyy") : "N/A")}";
                worksheet.Cells[2, 3, 2, 3].Merge = true;

                worksheet.Cells[4, 1].Value = "Tháng";
                worksheet.Cells[4, 2].Value = "Doanh thu đơn dịch vụ lẻ đã đặt (VNĐ)";
                worksheet.Cells[4, 3].Value = "Doanh thu hợp đồng (VNĐ)";
                worksheet.Cells[4, 4].Value = "Tổng doanh thu (VNĐ)";
                worksheet.Cells[4, 1, 4, 4].Style.Font.Bold = true;
                worksheet.Cells[4, 1, 4, 4].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                worksheet.Cells[4, 1, 4, 4].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);

                int row = 5;
                foreach (var item in model.MonthlyRevenueDetails)
                {
                    worksheet.Cells[row, 1].Value = item.Month;
                    worksheet.Cells[row, 2].Value = item.OrderRevenue;
                    worksheet.Cells[row, 3].Value = item.ContractRevenue;
                    worksheet.Cells[row, 4].Value = item.TotalRevenue;
                    row++;
                }

                // Định dạng cột
                if (row > 5) // Chỉ định dạng nếu có từ 1 dòng dữ liệu trở lên
                {
                    worksheet.Cells[5, 2, row - 1, 5].Style.Numberformat.Format = "#.##0";
                    worksheet.Cells[4, 1, row - 1, 6].AutoFitColumns();
                }
                else
                {
                    worksheet.Cells[4, 1, row - 1, 6].AutoFitColumns();
                }

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

            // Tạo file Excel
            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Doanh thu theo năm");

                worksheet.Cells[1, 1].Value = "BÁO CÁO DOANH THU THEO NĂM";
                worksheet.Cells[1, 1, 1, 3].Merge = true;
                worksheet.Cells[1, 1].Style.Font.Bold = true;
                worksheet.Cells[1, 1].Style.Font.Size = 16;
                worksheet.Cells[1, 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                worksheet.Cells[2, 1].Value = $"Từ ngày: {(model.StartDate.HasValue ? model.StartDate.Value.ToString("dd/MM/yyyy") : "N/A")}";
                worksheet.Cells[2, 1, 2, 2].Merge = true;
                worksheet.Cells[2, 3].Value = $"Đến ngày: {(model.EndDate.HasValue ? model.EndDate.Value.ToString("dd/MM/yyyy") : "N/A")}";
                worksheet.Cells[2, 3, 2, 3].Merge = true;

                worksheet.Cells[4, 1].Value = "Năm";
                worksheet.Cells[4, 2].Value = "Doanh thu đơn dịch vụ lẻ đã đặt (VNĐ)";
                worksheet.Cells[4, 3].Value = "Doanh thu hợp đồng (VNĐ)";
                worksheet.Cells[4, 4].Value = "Tổng doanh thu (VNĐ)";
                worksheet.Cells[4, 1, 4, 4].Style.Font.Bold = true;
                worksheet.Cells[4, 1, 4, 4].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                worksheet.Cells[4, 1, 4, 4].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);

                int row = 5;
                foreach (var item in model.YearlyRevenueDetails)
                {
                    worksheet.Cells[row, 1].Value = item.Year;
                    worksheet.Cells[row, 2].Value = item.OrderRevenue;
                    worksheet.Cells[row, 3].Value = item.ContractRevenue;
                    worksheet.Cells[row, 4].Value = item.TotalRevenue;
                    row++;
                }

                // Định dạng cột
                if (row > 5) // Chỉ định dạng nếu có từ 1 dòng dữ liệu trở lên
                {
                    worksheet.Cells[5, 2, row - 1, 5].Style.Numberformat.Format = "#.##0";
                    worksheet.Cells[4, 1, row - 1, 6].AutoFitColumns();
                }
                else
                {
                    worksheet.Cells[4, 1, row - 1, 6].AutoFitColumns();
                }

                var stream = new MemoryStream();
                package.SaveAs(stream);
                stream.Position = 0;
                string excelName = $"BaoCaoDoanhThu_Nam_{startDate?.ToString("yyyyMMdd")}_{endDate?.ToString("yyyyMMdd")}.xlsx";
                return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", excelName);
            }
        }
    }
}