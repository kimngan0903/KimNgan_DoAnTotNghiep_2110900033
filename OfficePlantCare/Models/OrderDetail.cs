using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace OfficePlantCare.Models;

public partial class OrderDetail
{
    public int OrderDetailId { get; set; }

    [Display(Name = "Mã đơn hàng")]
    public int OrderId { get; set; }

    [Display(Name = "Mã dịch vụ")]
    public int ServiceId { get; set; }

    [Display(Name = "Địa chỉ")]
    public string? Address { get; set; }

    [Display(Name = "Số lượng")]
    public int Quantity { get; set; }

    [Display(Name = "Mã giá")]
    public int PriceId { get; set; }

    [Display(Name = "Tổng tiền")]
    public decimal? TotalAmount { get; set; }

    [Display(Name = "Ghi chú")]
    public string? Notes { get; set; }

    [Display(Name = "Trạng thái")]
    public string? Status { get; set; }

    [Display(Name = "Đơn hàng")]
    public virtual Order? Order { get; set; } = null!;

    [Display(Name = "Giá dịch vụ")]
    public virtual ServicePrice? Price { get; set; } = null!;

    [Display(Name = "Dịch vụ")]
    public virtual Service? Service { get; set; } = null!;
}
