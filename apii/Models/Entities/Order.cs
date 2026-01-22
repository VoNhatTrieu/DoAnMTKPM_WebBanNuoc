namespace apii.Models.Entities;

/// <summary>
/// Entity Order - Đơn hàng
/// </summary>
public class Order
{
    public int Id { get; set; }
    public string OrderNumber { get; set; } = string.Empty;
    public int? UserId { get; set; }
    public DateTime OrderDate { get; set; } = DateTime.Now;
    public string Status { get; set; } = "Pending"; // Pending, Confirmed, Preparing, Shipping, Delivered, Cancelled
    public string? PaymentMethod { get; set; } // COD, MoMo, Banking, VNPay
    public string PaymentStatus { get; set; } = "Pending"; // Pending, Paid, Failed

    // Customer Info
    public string CustomerName { get; set; } = string.Empty;
    public string CustomerPhone { get; set; } = string.Empty;
    public string? CustomerEmail { get; set; }
    public string ShippingAddress { get; set; } = string.Empty;
    public string? Notes { get; set; }

    // Pricing
    public decimal Subtotal { get; set; }
    public decimal ShippingFee { get; set; }
    public decimal Discount { get; set; }
    public decimal Total { get; set; }

    // Navigation properties
    public virtual User? User { get; set; }
    public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();
}
