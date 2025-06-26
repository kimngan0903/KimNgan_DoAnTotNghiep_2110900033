using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace OfficePlantCare.Models;

public partial class Service
{
    [Display(Name = "Mã dịch vụ")]
    public int ServiceId { get; set; }

    [Display(Name = "Tên dịch vụ")]
    public string ServiceName { get; set; } = null!;

    [Display(Name = "Ngày tạo")]
    [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
    public DateTime? CreatedDate { get; set; }

    [Display(Name = "Ngày cập nhật")]
    [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
    public DateTime? UpdatedDate { get; set; }

    [Display(Name = "Trạng thái")]
    public string? Status { get; set; }

    [Display(Name = "Mã loại dịch vụ")]
    public int? CategoryServiceId { get; set; }

    [Display(Name = "Hình ảnh")]
    public string? Image { get; set; }

    [Display(Name = "Giỏ hàng")]
    public virtual ICollection<Cart> Carts { get; set; } = new List<Cart>();

    [Display(Name = "Loại dịch vụ")]
    public virtual ServiceCategory? CategoryService { get; set; }

    [Display(Name = "Chi tiết hợp đồng")]
    public virtual ICollection<ContractDetail> ContractDetails { get; set; } = new List<ContractDetail>();

    [Display(Name = "Phản hồi")]
    public virtual ICollection<Feedback> Feedbacks { get; set; } = new List<Feedback>();

    [Display(Name = "Chi tiết đơn hàng")]
    public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();

    [Display(Name = "Mô tả dịch vụ")]
    public virtual ICollection<ServiceDescription> ServiceDescriptions { get; set; } = new List<ServiceDescription>();

    [Display(Name = "Giá dịch vụ")]
    public virtual ICollection<ServicePrice> ServicePrices { get; set; } = new List<ServicePrice>();
}
