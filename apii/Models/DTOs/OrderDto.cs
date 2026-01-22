namespace apii.Models.DTOs;

/// <summary>
/// DTO cho tạo đơn hàng
/// </summary>
public class CreateOrderDto
{
    public string CustomerName { get; set; } = string.Empty;
    public string CustomerPhone { get; set; } = string.Empty;
    public string? CustomerEmail { get; set; }
    public string ShippingAddress { get; set; } = string.Empty;
    public string? Notes { get; set; }
    public string PaymentMethod { get; set; } = "COD";
    public List<OrderItemDto> Items { get; set; } = new List<OrderItemDto>();
}

/// <summary>
/// DTO cho Order Item
/// </summary>
public class OrderItemDto
{
    public int ProductId { get; set; }
    public int Quantity { get; set; }
    public string? Size { get; set; }
    public string? SugarLevel { get; set; }
    public string? IceLevel { get; set; }
    public string? Toppings { get; set; }
}

/// <summary>
/// DTO cho Order Response
/// </summary>
public class OrderDto
{
    public int Id { get; set; }
    public string OrderNumber { get; set; } = string.Empty;
    public DateTime OrderDate { get; set; }
    public string Status { get; set; } = string.Empty;
    public string PaymentMethod { get; set; } = string.Empty;
    public string PaymentStatus { get; set; } = string.Empty;
    public string CustomerName { get; set; } = string.Empty;
    public string CustomerPhone { get; set; } = string.Empty;
    public string? CustomerEmail { get; set; }
    public string ShippingAddress { get; set; } = string.Empty;
    public string? Notes { get; set; }
    public decimal Subtotal { get; set; }
    public decimal ShippingFee { get; set; }
    public decimal Discount { get; set; }
    public decimal Total { get; set; }
    public List<OrderDetailDto>? OrderDetails { get; set; }
}

/// <summary>
/// DTO cho Order Detail
/// </summary>
public class OrderDetailDto
{
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string? ImageUrl { get; set; }
    public decimal UnitPrice { get; set; }
    public int Quantity { get; set; }
    public string? Size { get; set; }
    public string? SugarLevel { get; set; }
    public string? IceLevel { get; set; }
    public string? Toppings { get; set; }
    public decimal TotalPrice { get; set; }
}
