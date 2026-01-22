namespace apii.Models.Entities;

/// <summary>
/// Entity Cart - Giỏ hàng
/// </summary>
public class Cart
{
    public int Id { get; set; }
    public int? UserId { get; set; }
    public string? SessionId { get; set; } // Cho khách vãng lai
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string? ImageUrl { get; set; }
    public decimal UnitPrice { get; set; }
    public int Quantity { get; set; }
    public string? Size { get; set; }
    public string? SugarLevel { get; set; }
    public string? IceLevel { get; set; }
    public string? Toppings { get; set; } // JSON or comma-separated
    public DateTime CreatedDate { get; set; } = DateTime.Now;

    // Navigation properties
    public virtual User? User { get; set; }
    public virtual Product Product { get; set; } = null!;
}
