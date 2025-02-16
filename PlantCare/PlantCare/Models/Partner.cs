using System;
using System.Collections.Generic;

namespace PlantCare.Models;

public partial class Partner
{
    public int PartnerId { get; set; }

    public string PartnerName { get; set; } = null!;

    public string? Logo { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? Description { get; set; }

    public string? Status { get; set; }
}
