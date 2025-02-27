using System;
using System.Collections.Generic;

namespace OfficePlantCare.Models;

public partial class Banner
{
    public int BannerId { get; set; }

    public string? Image { get; set; }

    public string? Title { get; set; }

    public DateTime? CreatedDate { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public string? Status { get; set; }

    public string? Description { get; set; }
}
