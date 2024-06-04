using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WebApplication3;

public partial class Order
{
    public int OrderId { get; set; }

    [Required(ErrorMessage = "Статус заказа обязателен")]
    public string OrderStatus { get; set; } = null!;

    [Required(ErrorMessage = "Номер пункта выдачи обязателен")]
    public DateTime OrderDeliveryDate { get; set; }

    public int OrderPickupPoint { get; set; }

    public DateTime? DateOrders { get; set; }

    [Required(ErrorMessage = "Имя клиента обязательно")]
    public string? NameClient { get; set; }

    public string? Code { get; set; }

    public virtual PickupPoint OrderPickupPointNavigation { get; set; } = null!;

    public virtual ICollection<OrderProduct> OrderProducts { get; } = new List<OrderProduct>();
}
