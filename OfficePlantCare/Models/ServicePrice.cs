using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace OfficePlantCare.Models;

public partial class ServicePrice
{
    [Display(Name = "Mã giá")]
    public int PriceId { get; set; }

    [Display(Name = "Mã dịch vụ")]
    public int? ServiceId { get; set; }

    [Display(Name = "Loại dịch vụ")]
    public string? ServiceType { get; set; }

    [Display(Name = "Kích thước cây")]
    public string? TreeSize { get; set; }

    [Display(Name = "Kích thước văn phòng")]
    public string? OfficeSize { get; set; }

    [Display(Name = "Thời gian (tháng)")]
    public int? DurationInMonths { get; set; }

    [Display(Name = "Số lượng cây")]
    public int? NumberOfTrees { get; set; }

    [Display(Name = "Giá")]
    public decimal? Price { get; set; }

    [Display(Name = "Dịch vụ")]
    public virtual Service? Service { get; set; }

    [Display(Name = "Chi tiết hợp đồng")]
    public virtual ICollection<ContractDetail> ContractDetails { get; set; } = new List<ContractDetail>();

    [Display(Name = "Chi tiết đơn hàng")]
    public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();
}
