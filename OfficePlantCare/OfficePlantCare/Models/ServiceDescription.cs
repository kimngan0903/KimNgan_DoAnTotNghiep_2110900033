using System;
using System.Collections.Generic;

namespace OfficePlantCare.Models;

public partial class ServiceDescription
{
    public int DescriptionId { get; set; }

    public int ServiceId { get; set; }

    public string? Image { get; set; }

    public int StepNumber { get; set; }

    public string Title { get; set; } = null!;

    public string Content { get; set; } = null!;

    public DateTime? CreatedDate { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public virtual Service Service { get; set; } = null!;
}
