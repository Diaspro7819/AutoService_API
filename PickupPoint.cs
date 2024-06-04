using System;
using System.Collections.Generic;

namespace WebApplication3;

public partial class PickupPoint
{
    public int Id { get; set; }

    public string? Address { get; set; }

    public virtual ICollection<Order> Orders { get; } = new List<Order>();
}
