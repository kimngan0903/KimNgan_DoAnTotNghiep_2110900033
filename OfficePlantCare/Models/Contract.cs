using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace OfficePlantCare.Models;

public partial class Contract
{
    public int ContractId { get; set; }

    [Display(Name = "Mã hợp đồng")]
    public string? ContractCode { get; set; }

    [Display(Name = "Mã khách hàng")]
    public int CustomerId { get; set; }

    [Display(Name = "Ngày tạo")]
    [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
    public DateTime? CreatedDate { get; set; }

    [Display(Name = "Tổng tiền")]
    public decimal? TotalAmount { get; set; }

    [Display(Name = "Đơn vị thời hạn")]
    public string DurationUnit { get; set; } = null!;

    [Display(Name = "Thời hạn")]
    public int Duration { get; set; }

    [Display(Name = "Ngày bắt đầu")]
    [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
    public DateOnly StartDate { get; set; }

    [Display(Name = "Ngày kết thúc")]
    [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
    public DateOnly? EndDate { get; set; }

    [Display(Name = "Mã phương thức thanh toán")]
    public int? PaymentMethodId { get; set; }

    [Display(Name = "Trạng thái thanh toán")]
    public string? PaymentStatus { get; set; }

    [Display(Name = "Trạng thái")]
    public string? Status { get; set; }

    [Display(Name = "Lịch chăm sóc")]
    public virtual ICollection<CareSchedule> CareSchedules { get; set; } = new List<CareSchedule>();

    [Display(Name = "Chi tiết hợp đồng")]
    public virtual ICollection<ContractDetail> ContractDetails { get; set; } = new List<ContractDetail>();

    [Display(Name = "Thông tin khách hàng")]
    public virtual Customer? Customer { get; set; } = null!;

    [Display(Name = "Phương thức thanh toán")]
    public virtual PaymentMethod? PaymentMethod { get; set; }
}