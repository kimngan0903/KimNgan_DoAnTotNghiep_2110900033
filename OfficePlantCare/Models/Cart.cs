using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace OfficePlantCare.Models;

public partial class Cart
{
    public int CartId { get; set; }

    [Display(Name = "Mã khách hàng")]
    public int CustomerId { get; set; }

    [Display(Name = "Mã dịch vụ")]
    public int ServiceId { get; set; }

    [Display(Name = "Ảnh")]
    public string? Image { get; set; }

    [Display(Name = "Số lượng")]
    public int Quantity { get; set; }

    [Display(Name = "Tổng tiền")]
    public decimal Total { get; set; }

    [Display(Name = "Loại dịch vụ")]
    public string? ServiceType { get; set; }

    [Display(Name = "Kích thước cây")]
    public string? TreeSize { get; set; }

    [Display(Name = "Kích thước văn phòng")]
    public string? OfficeSize { get; set; }

    [Display(Name = "Thời hạn (tháng)")]
    public int? DurationInMonths { get; set; }

    [Display(Name = "Số lượng cây")]
    public int? NumberOfTrees { get; set; }

    [Display(Name = "Địa chỉ")]
    public string? Address { get; set; }

    [Display(Name = "Ghi chú")]
    public string? Notes { get; set; }

    [Display(Name = "Ngày tạo")]
    [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
    public DateTime? CreatedDate { get; set; }

    [Display(Name = "Đơn giá")]
    public decimal Price { get; set; }

    [Display(Name = "Thông tin khách hàng")]
    public virtual Customer? Customer { get; set; } = null!;

    [Display(Name = "Thông tin dịch vụ")]
    public virtual Service? Service { get; set; } = null!;
}