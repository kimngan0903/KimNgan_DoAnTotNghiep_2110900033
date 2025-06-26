using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace OfficePlantCare.Models;

public partial class ServiceDescription
{
    [Display(Name = "Mã mô tả")]
    public int DescriptionId { get; set; }

    [Display(Name = "Mã dịch vụ")]
    public int ServiceId { get; set; }

    [Display(Name = "Hình ảnh")]
    public string? Image { get; set; }

    [Display(Name = "Số bước")]
    public int StepNumber { get; set; }

    [Display(Name = "Tiêu đề")]
    public string Title { get; set; } = null!;

    [Display(Name = "Nội dung")]
    public string Content { get; set; } = null!;

    [Display(Name = "Ngày tạo")]
    [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
    public DateTime? CreatedDate { get; set; }

    [Display(Name = "Ngày cập nhật")]
    [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
    public DateTime? UpdatedDate { get; set; }

    [Display(Name = "Dịch vụ")]
    public virtual Service? Service { get; set; } = null!;
}
