using System;
using System.Collections.Generic;

namespace OfficePlantCare.Models;

public partial class OrderDetail
{
    public int OrderDetailId { get; set; }

    public int? OrderId { get; set; }

    public int? ServiceId { get; set; }

    public string? Address { get; set; }

    public int Quantity { get; set; }

    public decimal Price { get; set; }

    public decimal? TotalAmount { get; set; }

    public string? Status { get; set; }

    public virtual Order? Order { get; set; }
}
