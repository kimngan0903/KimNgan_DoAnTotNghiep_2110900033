using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
namespace OfficePlantCare.Models;
public partial class Role
{
    [Display(Name = "Mã vai trò")]
    public int RoleId { get; set; }
    [Display(Name = "Tên vai trò")]
    public string RoleName { get; set; } = null!;
    [Display(Name = "Quản trị viên")]
    public virtual ICollection<Admin> Admins { get; set; } = new List<Admin>();
    [Display(Name = "Nhân viên")]
    public virtual ICollection<Staff> Staff { get; set; } = new List<Staff>();
}
