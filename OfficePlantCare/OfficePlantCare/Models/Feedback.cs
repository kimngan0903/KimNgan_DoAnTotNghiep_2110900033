using System;
using System.Collections.Generic;

namespace OfficePlantCare.Models;

public partial class Feedback
{
    public int FeedbackId { get; set; }

    public int? CustomerId { get; set; }

    public int? ServiceId { get; set; }

    public DateTime? FeedbackDate { get; set; }

    public int? Rating { get; set; }

    public string? Content { get; set; }

    public string? Status { get; set; }

    public virtual Customer? Customer { get; set; }

    public virtual Service? Service { get; set; }
}
