using System;
using System.Collections.Generic;

namespace OfficePlantCare.Models;

public partial class Staff
{
    public int StaffId { get; set; }

    public string StaffName { get; set; } = null!;

    public string? Position { get; set; }

    public string Phone { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string? Avatar { get; set; }

    public DateOnly? BirthDate { get; set; }

    public DateTime? HireDate { get; set; }

    public decimal? Salary { get; set; }

    public string? Status { get; set; }

    public int? RoleId { get; set; }

    public virtual ICollection<CareSchedule> CareSchedules { get; set; } = new List<CareSchedule>();

    public virtual Role? Role { get; set; }
}
