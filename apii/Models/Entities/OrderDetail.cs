namespace apii.Models.Entities;

/// <summary>
/// Entity OrderDetail - Chi tiết đơn hàng
/// </summary>
public class OrderDetail
{
    public int Id { get; set; }
    public int OrderId { get; set; }
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string? ImageUrl { get; set; }
    public decimal UnitPrice { get; set; }
    public int Quantity { get; set; }
    public string? Size { get; set; }
    public string? SugarLevel { get; set; }
    public string? IceLevel { get; set; }
    public string? Toppings { get; set; } // JSON or comma-separated
    public decimal TotalPrice { get; set; }

    // Navigation properties
    public virtual Order Order { get; set; } = null!;
    public virtual Product Product { get; set; } = null!;
}
