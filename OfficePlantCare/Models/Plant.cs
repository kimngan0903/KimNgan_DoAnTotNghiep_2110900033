using System;
using System.Collections.Generic;

namespace OfficePlantCare.Models;

public partial class Plant
{
    public int PlantId { get; set; }

    public string PlantName { get; set; } = null!;

    public string? Type { get; set; }

    public string? Size { get; set; }

    public string? HealthStatus { get; set; }

    public int? LocationId { get; set; }

    public string? Image { get; set; }

    public DateTime? CreatedDate { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public string? Notes { get; set; }

    public virtual ICollection<CareSchedule> CareSchedules { get; set; } = new List<CareSchedule>();

    public virtual Location? Location { get; set; }
}
