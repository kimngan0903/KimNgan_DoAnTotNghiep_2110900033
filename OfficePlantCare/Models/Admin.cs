using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace OfficePlantCare.Models;

public partial class Admin
{
    public int AdminId { get; set; }

    [Display(Name = "Tên tài khoản")]
    public string Username { get; set; } = null!;

    [Display(Name = "Mật khẩu")]
    public string PasswordHash { get; set; } = null!;

    [Display(Name = "Email")]
    public string Email { get; set; } = null!;

    [Display(Name = "Ngày tạo")]
    [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
    public DateTime? CreatedDate { get; set; }

    [Display(Name = "Ngày cập nhật")]
    [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
    public DateTime? UpdatedDate { get; set; }

    [Display(Name = "Ảnh đại diện")]
    public string? Avatar { get; set; }

    [Display(Name = "Trạng thái")]
    public string? Status { get; set; }

    [Display(Name = "Mã vai trò")]
    public int RoleId { get; set; }

    [Display(Name = "Địa chỉ")]
    public string? Address { get; set; }

    [Display(Name = "Thông báo")]
    public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();

    [Display(Name = "Thông tin vai trò")]
    public virtual Role? Role { get; set; } = null!;
}