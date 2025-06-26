using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace OfficePlantCare.Models;

public partial class PaymentMethod
{
    [Display(Name = "Mã phương thức thanh toán")]
    public int PaymentMethodId { get; set; }

    [Display(Name = "Tên phương thức thanh toán")]
    public string MethodName { get; set; } = null!;

    [Display(Name = "Hợp đồng")]
    public virtual ICollection<Contract> Contracts { get; set; } = new List<Contract>();

    [Display(Name = "Đơn hàng")]
    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}
