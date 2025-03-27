using System;
using System.Collections.Generic;

namespace OfficePlantCare.Models;

public partial class OrderDetail
{
    public int OrderDetailId { get; set; }

    public int OrderId { get; set; }

    public int ServiceId { get; set; }

    public string? Address { get; set; }

    public int Quantity { get; set; }

    public int PriceId { get; set; }

    public decimal? TotalAmount { get; set; }

    public string? Notes { get; set; }

    public string? Status { get; set; }

    public virtual Order? Order { get; set; } = null!;

    public virtual ServicePrice? Price { get; set; } = null!;

    public virtual Service? Service { get; set; } = null!;
}
