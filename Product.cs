using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WebApplication3;

public partial class Product
{
    [Required(ErrorMessage = "Артикул продукта обязателен")]
    public string ProductArticleNumber { get; set; } = null!;

    public string? ProductName { get; set; }

    public string? ProductDescription { get; set; }

    public string? ProductCategory { get; set; }

    public string? ProductPhoto { get; set; }

    public string? ProductManufacturer { get; set; }

    [Range(0, double.MaxValue, ErrorMessage = "Стоимость продукта должна быть больше или равна 0")]
    public decimal? ProductCost { get; set; }

    public int? ProductDiscountAmount { get; set; }

    [Range(0, int.MaxValue, ErrorMessage = "Количество на складе должно быть больше или равно 0")]
    public int? ProductQuantityInStock { get; set; }

    public string? ProductStatus { get; set; }

    public string? Unit { get; set; }

    public string? MaxDiscountAmount { get; set; }

    public string? Supplier { get; set; }

    public string? CountInPack { get; set; }

    [RegularExpression("^[0-9]+$", ErrorMessage = "Минимальное количество должно быть числом")]
    public string? MinCount { get; set; }

    public virtual ICollection<OrderProduct> OrderProducts { get; } = new List<OrderProduct>();
}
