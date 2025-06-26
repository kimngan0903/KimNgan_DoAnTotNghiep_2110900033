using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace OfficePlantCare.Models;

public partial class Notification
{
    public int NotificationId { get; set; }

    [Display(Name = "Mã admin")]
    public int AdminId { get; set; }

    [Display(Name = "Mã tham chiếu")]
    public string ReferenceId { get; set; } = null!;

    [Display(Name = "Loại")]
    public string Type { get; set; } = null!;

    [Display(Name = "Nội dung")]
    public string Message { get; set; } = null!;

    [Display(Name = "Ngày tạo")]
    [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
    public DateTime CreatedDate { get; set; }

    [Display(Name = "Đã đọc")]
    public bool IsRead { get; set; }

    [Display(Name = "Đã xóa")]
    public bool IsDeleted { get; set; }

    [Display(Name = "Thông tin admin")]
    public virtual Admin Admin { get; set; } = null!;
}