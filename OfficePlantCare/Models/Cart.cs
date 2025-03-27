using System;
using System.Collections.Generic;

namespace OfficePlantCare.Models;

public partial class Cart
{
    public int CartId { get; set; }

    public int CustomerId { get; set; }

    public int ServiceId { get; set; }

    public string ServiceName { get; set; } = null!;

    public string? Image { get; set; }

    public int Quantity { get; set; }

    public decimal Total { get; set; }

    public string? ServiceType { get; set; }

    public string? TreeSize { get; set; }

    public string? OfficeSize { get; set; }

    public int? DurationInMonths { get; set; }

    public int? NumberOfTrees { get; set; }

    public string? Address { get; set; }

    public string? Notes { get; set; }

    public DateTime? CreatedDate { get; set; }

    public decimal Price { get; set; }
    public virtual Service? Service { get; set; }
}
