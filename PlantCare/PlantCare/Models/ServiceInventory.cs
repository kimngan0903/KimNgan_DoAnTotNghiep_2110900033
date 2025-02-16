using System;
using System.Collections.Generic;

namespace PlantCare.Models;

public partial class ServiceInventory
{
    public int ServiceInventoryId { get; set; }

    public int ServiceId { get; set; }

    public int ItemId { get; set; }

    public decimal QuantityRequired { get; set; }

    public string? Notes { get; set; }

    public virtual InventoryItem Item { get; set; } = null!;

    public virtual Service Service { get; set; } = null!;
}
