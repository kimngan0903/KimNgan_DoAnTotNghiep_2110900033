using System;
using System.Collections.Generic;

namespace OfficePlantCare.Models;

public partial class CareScheduleSupply
{
    public int CareScheduleSupplyId { get; set; }

    public int ScheduleId { get; set; }

    public int SupplyId { get; set; }

    public decimal QuantityUsed { get; set; }

    public string? Notes { get; set; }

    public virtual CareSchedule? Schedule { get; set; } = null!;

    public virtual Supply? Supply { get; set; } = null!;
}
