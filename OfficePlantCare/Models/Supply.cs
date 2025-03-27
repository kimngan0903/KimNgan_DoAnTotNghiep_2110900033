using System;
using System.Collections.Generic;

namespace OfficePlantCare.Models;

public partial class Supply
{
    public int SupplyId { get; set; }

    public string SupplyName { get; set; } = null!;

    public string? Unit { get; set; }

    public decimal Quantity { get; set; }

    public DateTime? CreatedDate { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public string? Status { get; set; }

    public virtual ICollection<CareScheduleSupply> CareScheduleSupplies { get; set; } = new List<CareScheduleSupply>();
}
