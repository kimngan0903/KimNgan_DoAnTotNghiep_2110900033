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
        public DateTime? StartDate { get; set; } // Thêm để lưu khoảng thời gian lọc
        public DateTime? EndDate { get; set; }   // Thêm để lưu khoảng thời gian lọc
        public List<DailyRevenue> DailyRevenue { get; set; }
        public List<MonthlyRevenue> MonthlyRevenue { get; set; }
        public List<YearlyRevenue> YearlyRevenue { get; set; }
        public List<DailyRevenueDetail> DailyRevenueDetails { get; set; }
        public List<MonthlyRevenueDetail> MonthlyRevenueDetails { get; set; }
        public List<YearlyRevenueDetail> YearlyRevenueDetails { get; set; }

        // Khởi tạo các danh sách để tránh lỗi null
        public RevenueReport()
        {
            DailyRevenue = new List<DailyRevenue>();
            MonthlyRevenue = new List<MonthlyRevenue>();
            YearlyRevenue = new List<YearlyRevenue>();
            DailyRevenueDetails = new List<DailyRevenueDetail>();
            MonthlyRevenueDetails = new List<MonthlyRevenueDetail>();
            YearlyRevenueDetails = new List<YearlyRevenueDetail>();
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

    public class ServiceUsageDetail
    {
        public string ServiceName { get; set; }
        public int Quantity { get; set; }
        public decimal Revenue { get; set; }
    }

    public class DailyRevenueDetail
    {
        public string Day { get; set; }
        public DateTime OriginalDate { get; set; }
        public decimal OrderRevenue { get; set; }
        public decimal ServiceRequestRevenue { get; set; }
        public decimal ContractRevenue { get; set; }
        public decimal TotalRevenue => OrderRevenue + ServiceRequestRevenue + ContractRevenue;
        public List<ServiceUsageDetail> ServiceUsageDetails { get; set; } // Thêm danh sách dịch vụ được sử dụng

        public DailyRevenueDetail()
        {
            ServiceUsageDetails = new List<ServiceUsageDetail>();
        }
    }

    public class MonthlyRevenueDetail
    {
        public string Month { get; set; }
        public DateTime OriginalDate { get; set; }
        public decimal OrderRevenue { get; set; }
        public decimal ServiceRequestRevenue { get; set; }
        public decimal ContractRevenue { get; set; }
        public decimal TotalRevenue => OrderRevenue + ServiceRequestRevenue + ContractRevenue;
        public List<ServiceUsageDetail> ServiceUsageDetails { get; set; } // Thêm danh sách dịch vụ được sử dụng

        public MonthlyRevenueDetail()
        {
            ServiceUsageDetails = new List<ServiceUsageDetail>();
        }
    }

    public class YearlyRevenueDetail
    {
        public string Year { get; set; }
        public int OriginalYear { get; set; }
        public decimal OrderRevenue { get; set; }
        public decimal ServiceRequestRevenue { get; set; }
        public decimal ContractRevenue { get; set; }
        public decimal TotalRevenue => OrderRevenue + ServiceRequestRevenue + ContractRevenue;
        public List<ServiceUsageDetail> ServiceUsageDetails { get; set; } // Thêm danh sách dịch vụ được sử dụng

        public YearlyRevenueDetail()
        {
            ServiceUsageDetails = new List<ServiceUsageDetail>();
        }
    }
}