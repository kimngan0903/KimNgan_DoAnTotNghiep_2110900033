using System;
using System.Collections.Generic;

namespace OfficePlantCare.Models;

public partial class CareScheduleInventory
{
    public int CareScheduleInventoryId { get; set; }

    public int CareScheduleId { get; set; }

    public int ItemId { get; set; }

    public int QuantityUsed { get; set; }

    public string? Notes { get; set; }

    public virtual CareSchedule CareSchedule { get; set; } = null!;

    public virtual InventoryItem Item { get; set; } = null!;
}
