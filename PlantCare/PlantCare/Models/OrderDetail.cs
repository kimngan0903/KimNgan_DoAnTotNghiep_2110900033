using System;
using System.Collections.Generic;

namespace PlantCare.Models;

public partial class OrderDetail
{
    public int OrderDetailId { get; set; }

    public int? OrderId { get; set; }

    public int? ServiceId { get; set; }

    public int? ProductId { get; set; }

    public string? Address { get; set; }

    public int Quantity { get; set; }

    public decimal Price { get; set; }

    public decimal? TotalPrice { get; set; }

    public string? Status { get; set; }

    public virtual Order? Order { get; set; }

    public virtual Product? Product { get; set; }

    public virtual Service? Service { get; set; }
}
