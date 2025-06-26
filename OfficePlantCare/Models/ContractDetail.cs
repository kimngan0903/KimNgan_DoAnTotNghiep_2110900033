using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace OfficePlantCare.Models;

public partial class ContractDetail
{
    public int ContractDetailId { get; set; }

    [Display(Name = "Mã hợp đồng")]
    public int ContractId { get; set; }

    [Display(Name = "Mã dịch vụ")]
    public int ServiceId { get; set; }

    [Display(Name = "Địa chỉ")]
    public string? Address { get; set; }

    [Display(Name = "Mã giá")]
    public int PriceId { get; set; }

    [Display(Name = "Tổng tiền")]
    public decimal? TotalAmount { get; set; }

    [Display(Name = "Ghi chú")]
    public string? Notes { get; set; }

    [Display(Name = "Trạng thái")]
    public string? Status { get; set; }

    [Display(Name = "Thông tin hợp đồng")]
    public virtual Contract? Contract { get; set; } = null!;

    [Display(Name = "Thông tin giá")]
    public virtual ServicePrice? Price { get; set; } = null!;

    [Display(Name = "Thông tin dịch vụ")]
    public virtual Service? Service { get; set; } = null!;
}