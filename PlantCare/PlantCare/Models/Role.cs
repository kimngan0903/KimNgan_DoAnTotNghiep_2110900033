using System;
using System.Collections.Generic;

namespace PlantCare.Models;

public partial class Role
{
    public int RoleId { get; set; }

    public string RoleName { get; set; } = null!;

    public virtual ICollection<Admin> Admins { get; set; } = new List<Admin>();

    public virtual ICollection<Staff> Staff { get; set; } = new List<Staff>();
}
