using System;
using System.Collections.Generic;

namespace Web.Models;

public partial class Order
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public int Status { get; set; }

    public double Shipping { get; set; }

    public double Total { get; set; }

    public double Discount { get; set; }

    public double GrandTotal { get; set; }

    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

    public virtual User User { get; set; } = null!;
}
