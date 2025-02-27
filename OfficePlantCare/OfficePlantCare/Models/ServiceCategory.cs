using System;
using System.Collections.Generic;

namespace OfficePlantCare.Models;

public partial class ServiceCategory
{
    public int CategoryServiceId { get; set; }

    public string CategoryServiceName { get; set; } = null!;

    public string? Description { get; set; }

    public virtual ICollection<Service> Services { get; set; } = new List<Service>();
}
