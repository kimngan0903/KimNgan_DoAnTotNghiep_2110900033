using System;
using System.Collections.Generic;

namespace OfficePlantCare.Models;

public partial class ServiceRequest
{
    public int RequestId { get; set; }

    public int? CustomerId { get; set; }

    public int ServiceId { get; set; }

    public int? LocationId { get; set; }

    public int PriceId { get; set; }

    public int Quantity { get; set; }

    public decimal? TotalAmount { get; set; }

    public string? Notes { get; set; }

    public DateTime? RequestDate { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public string? Status { get; set; }

    public int PaymentMethodId { get; set; }

    public string PaymentStatus { get; set; } = null!;

    public virtual ICollection<CareSchedule> CareSchedules { get; set; } = new List<CareSchedule>();

    public virtual Customer? Customer { get; set; } = null!;

    public virtual Location? Location { get; set; }

    public virtual PaymentMethod? PaymentMethod { get; set; } = null!;

    public virtual ServicePrice? Price { get; set; } = null!;

    public virtual Service? Service { get; set; } = null!;
}
