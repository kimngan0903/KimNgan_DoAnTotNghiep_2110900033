using System;
using System.Collections.Generic;

namespace OfficePlantCare.Models;

public partial class Contract
{
    public int ContractId { get; set; }

    public string? ContractCode { get; set; }

    public int CustomerId { get; set; }

    public int ServiceId { get; set; }

    public int PriceId { get; set; }

    public string DurationUnit { get; set; } = null!;

    public int Duration { get; set; }

    public decimal? TotalAmount { get; set; }

    public DateTime? StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    public int? PaymentMethodId { get; set; }

    public string? PaymentStatus { get; set; }

    public string? Status { get; set; }

    public virtual ICollection<CareSchedule> CareSchedules { get; set; } = new List<CareSchedule>();

    public virtual Customer? Customer { get; set; } = null!;

    public virtual PaymentMethod? PaymentMethod { get; set; }

    public virtual ServicePrice? Price { get; set; } = null!;

    public virtual Service? Service { get; set; } = null!;
}
