using System;
using System.Collections.Generic;

namespace OfficePlantCare.Areas.AdminQL.Models
{
    public class RevenueReport
    {
        public int TotalCustomers { get; set; }
        public int TotalContracts { get; set; }
        public int TotalServices { get; set; }
        public decimal TotalRevenue { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public List<DailyRevenue> DailyRevenue { get; set; }
        public List<MonthlyRevenue> MonthlyRevenue { get; set; }
        public List<YearlyRevenue> YearlyRevenue { get; set; }
        public List<DailyRevenueDetail> DailyRevenueDetails { get; set; }
        public List<MonthlyRevenueDetail> MonthlyRevenueDetails { get; set; }
        public List<YearlyRevenueDetail> YearlyRevenueDetails { get; set; }
        public string ReportType { get; set; }

        // Thêm tổng số tiền theo phương thức thanh toán
        public decimal CashTotal { get; set; }
        public decimal VietQRTotal { get; set; }

        // Thêm chi tiết theo ngày/tháng/năm
        public List<DailyPaymentDetail> DailyPaymentDetails { get; set; }
        public List<MonthlyPaymentDetail> MonthlyPaymentDetails { get; set; }
        public List<YearlyPaymentDetail> YearlyPaymentDetails { get; set; }

        public RevenueReport()
        {
            DailyRevenue = new List<DailyRevenue>();
            MonthlyRevenue = new List<MonthlyRevenue>();
            YearlyRevenue = new List<YearlyRevenue>();
            DailyRevenueDetails = new List<DailyRevenueDetail>();
            MonthlyRevenueDetails = new List<MonthlyRevenueDetail>();
            YearlyRevenueDetails = new List<YearlyRevenueDetail>();
            DailyPaymentDetails = new List<DailyPaymentDetail>();
            MonthlyPaymentDetails = new List<MonthlyPaymentDetail>();
            YearlyPaymentDetails = new List<YearlyPaymentDetail>();
        }
    }

    public class DailyRevenue
    {
        public string Day { get; set; }
        public DateTime OriginalDate { get; set; }
        public decimal Revenue { get; set; }
    }

    public class MonthlyRevenue
    {
        public string Month { get; set; }
        public DateTime OriginalDate { get; set; }
        public decimal Revenue { get; set; }
    }

    public class YearlyRevenue
    {
        public string Year { get; set; }
        public int OriginalYear { get; set; }
        public decimal Revenue { get; set; }
    }

    public class DailyRevenueDetail
    {
        public string Day { get; set; }
        public DateTime OriginalDate { get; set; }
        public decimal OrderRevenue { get; set; }
        public decimal ContractRevenue { get; set; }
        public decimal TotalRevenue => OrderRevenue + ContractRevenue;
    }

    public class MonthlyRevenueDetail
    {
        public string Month { get; set; }
        public DateTime OriginalDate { get; set; }
        public decimal OrderRevenue { get; set; }
        public decimal ContractRevenue { get; set; }
        public decimal TotalRevenue => OrderRevenue + ContractRevenue;
    }

    public class YearlyRevenueDetail
    {
        public string Year { get; set; }
        public int OriginalYear { get; set; }
        public decimal OrderRevenue { get; set; }
        public decimal ContractRevenue { get; set; }
        public decimal TotalRevenue => OrderRevenue + ContractRevenue;
    }

    // Thêm các lớp chi tiết phương thức thanh toán
    public class DailyPaymentDetail
    {
        public string Day { get; set; }
        public DateTime OriginalDate { get; set; }
        public decimal CashAmount { get; set; }
        public decimal VietQRAmount { get; set; }
    }

    public class MonthlyPaymentDetail
    {
        public string Month { get; set; }
        public DateTime OriginalDate { get; set; }
        public decimal CashAmount { get; set; }
        public decimal VietQRAmount { get; set; }
    }

    public class YearlyPaymentDetail
    {
        public string Year { get; set; }
        public int OriginalYear { get; set; }
        public decimal CashAmount { get; set; }
        public decimal VietQRAmount { get; set; }
    }
}