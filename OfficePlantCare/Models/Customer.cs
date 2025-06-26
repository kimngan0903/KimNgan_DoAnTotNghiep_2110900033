using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace OfficePlantCare.Models;

public partial class Customer
{
    public int CustomerId { get; set; }

    [Display(Name = "Tên khách hàng")]
    public string CustomerName { get; set; } = null!;

    [Display(Name = "Email")]
    public string Email { get; set; } = null!;

    [Display(Name = "Số điện thoại")]
    public string Phone { get; set; } = null!;

    [Display(Name = "Địa chỉ")]
    public string? Address { get; set; }

    [Display(Name = "Mật khẩu")]
    public string PasswordHash { get; set; } = null!;

    [Display(Name = "Ngày tạo")]
    [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
    public DateTime? CreatedDate { get; set; }

    [Display(Name = "Trạng thái")]
    public string? Status { get; set; }

    [Display(Name = "Giỏ hàng")]
    public virtual ICollection<Cart> Carts { get; set; } = new List<Cart>();

    [Display(Name = "Hợp đồng")]
    public virtual ICollection<Contract> Contracts { get; set; } = new List<Contract>();

    [Display(Name = "Phản hồi")]
    public virtual ICollection<Feedback> Feedbacks { get; set; } = new List<Feedback>();

    [Display(Name = "Đơn hàng")]
    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}