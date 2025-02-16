using System;
using System.Collections.Generic;

namespace PlantCare.Models;

public partial class CareSchedule
{
    public int ScheduleId { get; set; }

    public int ContractId { get; set; }

    public int? StaffId { get; set; }

    public DateTime? ScheduledDate { get; set; }

    public DateTime? ActualDate { get; set; }

    public int Duration { get; set; }

    public string? Status { get; set; }

    public virtual ICollection<CareScheduleInventory> CareScheduleInventories { get; set; } = new List<CareScheduleInventory>();

    public virtual Contract Contract { get; set; } = null!;

    public virtual ICollection<ServiceRequest> ServiceRequests { get; set; } = new List<ServiceRequest>();

    public virtual Staff? Staff { get; set; }
}
