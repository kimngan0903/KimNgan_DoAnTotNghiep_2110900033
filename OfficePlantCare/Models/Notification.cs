using System;
using System.Collections.Generic;

namespace OfficePlantCare.Models;

public partial class Notification
{
    public int NotificationId { get; set; }

    public int AdminId { get; set; }

    public string ReferenceId { get; set; } = null!;

    public string Type { get; set; } = null!;

    public string Message { get; set; } = null!;

    public DateTime CreatedDate { get; set; }

    public bool IsRead { get; set; }

    public bool IsDeleted { get; set; }

    public virtual Admin? Admin { get; set; } = null!;
}
