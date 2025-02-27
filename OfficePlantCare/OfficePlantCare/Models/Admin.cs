using System;
using System.Collections.Generic;

namespace OfficePlantCare.Models;

public partial class Admin
{
    public int AdminId { get; set; }

    public string Username { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;

    public string Email { get; set; } = null!;

    public DateTime? CreatedDate { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public string? Avatar { get; set; }

    public string? Status { get; set; }

    public int RoleId { get; set; }

    public string? Address { get; set; }

    public virtual Role Role { get; set; } = null!;
}
