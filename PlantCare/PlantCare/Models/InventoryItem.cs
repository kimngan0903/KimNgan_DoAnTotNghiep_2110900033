using System;
using System.Collections.Generic;

namespace PlantCare.Models;

public partial class InventoryItem
{
    public int ItemId { get; set; }

    public string ItemName { get; set; } = null!;

    public string? Description { get; set; }

    public string? Unit { get; set; }

    public decimal? Price { get; set; }

    public int StockQuantity { get; set; }

    public string ItemType { get; set; } = null!;

    public int? CategoryId { get; set; }

    public string? Status { get; set; }

    public DateTime? CreatedDate { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public virtual ICollection<CareScheduleInventory> CareScheduleInventories { get; set; } = new List<CareScheduleInventory>();

    public virtual Category? Category { get; set; }

    public virtual ICollection<ServiceInventory> ServiceInventories { get; set; } = new List<ServiceInventory>();
}
