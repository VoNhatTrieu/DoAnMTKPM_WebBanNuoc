namespace apii.Models.DTOs;

/// <summary>
/// DTO cho thêm sản phẩm vào giỏ hàng
/// </summary>
public class AddToCartDto
{
    public int ProductId { get; set; }
    public int Quantity { get; set; } = 1;
    public string? Size { get; set; }
    public string? SugarLevel { get; set; }
    public string? IceLevel { get; set; }
    public List<string>? Toppings { get; set; }
}

/// <summary>
/// DTO cho Cart Item
/// </summary>
public class CartItemDto
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string? ImageUrl { get; set; }
    public decimal UnitPrice { get; set; }
    public int Quantity { get; set; }
    public string? Size { get; set; }
    public string? SugarLevel { get; set; }
    public string? IceLevel { get; set; }
    public string? Toppings { get; set; }
    public decimal TotalPrice => UnitPrice * Quantity;
}

/// <summary>
/// DTO cho Cart Summary
/// </summary>
public class CartSummaryDto
{
    public List<CartItemDto> Items { get; set; } = new List<CartItemDto>();
    public decimal Subtotal { get; set; }
    public decimal ShippingFee { get; set; }
    public decimal Discount { get; set; }
    public decimal Total { get; set; }
}
