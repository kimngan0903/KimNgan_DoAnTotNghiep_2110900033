using System;
using System.Collections.Generic;

namespace PlantCare.Models;

public partial class Contract
{
    public int ContractId { get; set; }

    public string? ContractCode { get; set; }

    public int CustomerId { get; set; }

    public int ContractDuration { get; set; }

    public DateOnly StartDate { get; set; }

    public DateOnly? EndDate { get; set; }

    public decimal ServiceFee { get; set; }

    public decimal? TotalAmount { get; set; }

    public string? Status { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<CareSchedule> CareSchedules { get; set; } = new List<CareSchedule>();

    public virtual Customer Customer { get; set; } = null!;
}
