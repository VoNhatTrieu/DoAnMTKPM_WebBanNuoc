using System;
using System.Collections.Generic;

namespace WebBanNuoc.Models.Entities
{
    /// <summary>
    /// Order - Đơn hàng
    /// </summary>
    public class Order
    {
        public int Id { get; set; }
        public string OrderNumber { get; set; }
        public int UserId { get; set; }
        public DateTime OrderDate { get; set; }
        public OrderStatus Status { get; set; }
        public PaymentMethod PaymentMethod { get; set; }
        public PaymentStatus PaymentStatus { get; set; }
        
        // Customer Info
        public string CustomerName { get; set; }
        public string CustomerPhone { get; set; }
        public string CustomerEmail { get; set; }
        public string ShippingAddress { get; set; }
        public string Notes { get; set; }

        // Pricing
        public decimal Subtotal { get; set; }
        public decimal ShippingFee { get; set; }
        public decimal Discount { get; set; }
        public decimal Total { get; set; }

        // Items
        public List<OrderDetail> OrderDetails { get; set; }

        public Order()
        {
            OrderDetails = new List<OrderDetail>();
            OrderDate = DateTime.Now;
            OrderNumber = GenerateOrderNumber();
            Status = OrderStatus.Pending;
            PaymentStatus = PaymentStatus.Pending;
        }

        private string GenerateOrderNumber()
        {
            return $"ORD{DateTime.Now:yyyyMMddHHmmss}";
        }
    }

    /// <summary>
    /// Order Detail - Chi tiết đơn hàng
    /// </summary>
    public class OrderDetail
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string ImageUrl { get; set; }
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }
        public string Size { get; set; }
        public string SugarLevel { get; set; }
        public string IceLevel { get; set; }
        public string Toppings { get; set; }
        public decimal TotalPrice { get; set; }
    }

    /// <summary>
    /// State Pattern - Trạng thái đơn hàng
    /// </summary>
    public enum OrderStatus
    {
        Pending,        // Chờ xác nhận
        Confirmed,      // Đã xác nhận
        Preparing,      // Đang pha chế
        Shipping,       // Đang giao
        Delivered,      // Đã giao
        Cancelled       // Đã hủy
    }

    /// <summary>
    /// Factory Pattern - Phương thức thanh toán
    /// </summary>
    public enum PaymentMethod
    {
        COD,            // Cash on Delivery
        MoMo,           // MoMo Wallet
        Banking,        // Internet Banking
        VNPay           // VNPay Gateway
    }

    /// <summary>
    /// Trạng thái thanh toán
    /// </summary>
    public enum PaymentStatus
    {
        Pending,        // Chờ thanh toán
        Paid,           // Đã thanh toán
        Failed,         // Thất bại
        Refunded        // Đã hoàn tiền
    }
}
