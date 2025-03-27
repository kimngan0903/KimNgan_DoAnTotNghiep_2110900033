using System;
using System.Collections.Generic;

namespace OfficePlantCare.Models;

public partial class ServicePrice
{
    public int PriceId { get; set; }

    public int? ServiceId { get; set; }

    public string? ServiceType { get; set; }

    public string? TreeSize { get; set; }

    public string? OfficeSize { get; set; }

    public int? DurationInMonths { get; set; }

    public int? NumberOfTrees { get; set; }

    public decimal? Price { get; set; }

    public virtual ICollection<Contract> Contracts { get; set; } = new List<Contract>();

    public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();

    public virtual Service? Service { get; set; }

    public virtual ICollection<ServiceRequest> ServiceRequests { get; set; } = new List<ServiceRequest>();
}
