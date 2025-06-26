using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace OfficePlantCare.Models;

public partial class ServiceCategory
{
    [Display(Name = "Mã danh mục")]
    public int CategoryServiceId { get; set; }

    [Display(Name = "Tên danh mục")]
    public string CategoryServiceName { get; set; } = null!;

    [Display(Name = "Dịch vụ")]
    public virtual ICollection<Service> Services { get; set; } = new List<Service>();
}
