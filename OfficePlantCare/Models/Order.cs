using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace OfficePlantCare.Models;

public partial class Order
{
    public int OrderId { get; set; }

    [Display(Name = "Mã khách hàng")]
    public int? CustomerId { get; set; }

    [Display(Name = "Mã nhân viên")]
    public int? StaffId { get; set; }

    [Display(Name = "Ngày đặt")]
    [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
    public DateTime? OrderDate { get; set; }

    [Display(Name = "Tổng giá")]
    public decimal? TotalPrice { get; set; }

    [Display(Name = "Mã phương thức thanh toán")]
    public int PaymentMethodId { get; set; }

    [Display(Name = "Ngày thanh toán")]
    [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
    public DateTime? PaymentDate { get; set; }

    [Display(Name = "Trạng thái thanh toán")]
    public string? PaymentStatus { get; set; }

    [Display(Name = "Trạng thái")]
    public string? Status { get; set; }

    [Display(Name = "Lịch chăm sóc")]
    public virtual ICollection<CareSchedule> CareSchedules { get; set; } = new List<CareSchedule>();

    [Display(Name = "Thông tin khách hàng")]
    public virtual Customer? Customer { get; set; }

    [Display(Name = "Chi tiết đơn hàng")]
    public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();

    [Display(Name = "Phương thức thanh toán")]
    public virtual PaymentMethod? PaymentMethod { get; set; } = null!;

    [Display(Name = "Thông tin nhân viên")]
    public virtual Staff? Staff { get; set; }
}