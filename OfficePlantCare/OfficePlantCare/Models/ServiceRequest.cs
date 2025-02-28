﻿using System;
using System.Collections.Generic;

namespace OfficePlantCare.Models;

public partial class ServiceRequest
{
    public int RequestId { get; set; }

    public int? ScheduleId { get; set; }

    public int? ServiceId { get; set; }

    public int? CustomerId { get; set; }

    public int Quantity { get; set; }

    public int PriceId { get; set; }

    public decimal? TotalAmount { get; set; }

    public string? Notes { get; set; }

    public DateTime? RequestDate { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public string? Status { get; set; }

    public virtual Customer? Customer { get; set; }

    public virtual ServicePrice Price { get; set; } = null!;

    public virtual CareSchedule? Schedule { get; set; }

    public virtual Service? Service { get; set; }
}
