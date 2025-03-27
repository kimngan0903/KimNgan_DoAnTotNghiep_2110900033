using System;
using System.Collections.Generic;

namespace OfficePlantCare.Models;

public partial class CareSchedule
{
    public int ScheduleId { get; set; }

    public int? ContractId { get; set; }

    public int? OrderId { get; set; }

    public int? RequestId { get; set; }

    public int? PlantId { get; set; }

    public int? StaffId { get; set; }

    public DateOnly ScheduledDate { get; set; }

    public TimeOnly ScheduledTime { get; set; }

    public DateOnly? ActualDate { get; set; }

    public decimal Duration { get; set; }

    public string? Status { get; set; }

    public virtual ICollection<CareScheduleSupply> CareScheduleSupplies { get; set; } = new List<CareScheduleSupply>();

    public virtual Contract? Contract { get; set; }

    public virtual Order? Order { get; set; }

    public virtual Plant? Plant { get; set; }

    public virtual ServiceRequest? Request { get; set; }

    public virtual Staff? Staff { get; set; }
}
