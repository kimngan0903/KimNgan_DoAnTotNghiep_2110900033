using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace OfficePlantCare.Models;

public partial class CareSchedule
{
    public int ScheduleId { get; set; }

    [Display(Name = "Mã hợp đồng")]
    public int? ContractId { get; set; }

    [Display(Name = "Mã đơn hàng")]
    public int? OrderId { get; set; }

    [Display(Name = "Mã nhân viên")]
    public int? StaffId { get; set; }

    [Display(Name = "Ngày dự kiến")]
    [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
    public DateOnly ScheduledDate { get; set; }

    [Display(Name = "Giờ dự kiến")]
    public TimeOnly ScheduledTime { get; set; }

    [Display(Name = "Ngày thực tế")]
    [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
    public DateOnly? ActualDate { get; set; }

    [Display(Name = "Thời lượng (giờ)")]
    public decimal Duration { get; set; }

    [Display(Name = "Trạng thái")]
    public string? Status { get; set; }

    [Display(Name = "Thông tin hợp đồng")]
    public virtual Contract? Contract { get; set; }

    [Display(Name = "Thông tin đơn hàng")]
    public virtual Order? Order { get; set; }

    [Display(Name = "Thông tin nhân viên")]
    public virtual Staff? Staff { get; set; }
}