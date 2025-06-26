using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace OfficePlantCare.Models;

public partial class Staff
{
    [Display(Name = "Mã nhân viên")]
    public int StaffId { get; set; }

    [Display(Name = "Tên nhân viên")]
    public string StaffName { get; set; } = null!;

    [Display(Name = "Chức vụ")]
    public string? Position { get; set; }

    [Display(Name = "Số điện thoại")]
    public string Phone { get; set; } = null!;

    [Display(Name = "Email")]
    public string Email { get; set; } = null!;

    [Display(Name = "Ảnh đại diện")]
    public string? Avatar { get; set; }

    [Display(Name = "Ngày sinh")]
    public DateOnly? BirthDate { get; set; }

    [Display(Name = "Ngày tuyển dụng")]
    [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
    public DateTime? HireDate { get; set; }

    [Display(Name = "Lương")]
    public decimal? Salary { get; set; }

    [Display(Name = "Trạng thái")]
    public string? Status { get; set; }

    [Display(Name = "Mã vai trò")]
    public int? RoleId { get; set; }

    [Display(Name = "Lịch chăm sóc")]
    public virtual ICollection<CareSchedule> CareSchedules { get; set; } = new List<CareSchedule>();

    [Display(Name = "Đơn hàng")]
    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

    [Display(Name = "Vai trò")]
    public virtual Role? Role { get; set; }
}
