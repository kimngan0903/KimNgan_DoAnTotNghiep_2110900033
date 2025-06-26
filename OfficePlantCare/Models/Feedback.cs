using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace OfficePlantCare.Models;

public partial class Feedback
{
    public int FeedbackId { get; set; }

    [Display(Name = "Mã khách hàng")]
    public int? CustomerId { get; set; }

    [Display(Name = "Mã dịch vụ")]
    public int? ServiceId { get; set; }

    [Display(Name = "Ngày phản hồi")]
    [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
    public DateTime? FeedbackDate { get; set; }

    [Display(Name = "Đánh giá (sao)")]
    public int? Rating { get; set; }

    [Display(Name = "Nội dung")]
    public string? Content { get; set; }

    [Display(Name = "Thông tin khách hàng")]
    public virtual Customer? Customer { get; set; }

    [Display(Name = "Thông tin dịch vụ")]
    public virtual Service? Service { get; set; }
}