using System.Collections.Generic;

namespace WebBanNuoc.Models.DTOs
{
    /// <summary>
    /// DTO cho việc tạo đơn hàng mới
    /// </summary>
    public class CreateOrderDTO
    {
        public string CustomerName { get; set; }
        public string CustomerPhone { get; set; }
        public string CustomerEmail { get; set; }
        public string ShippingAddress { get; set; }
        public string Notes { get; set; }
        public string PaymentMethod { get; set; }
        public string VoucherCode { get; set; }
        public List<OrderItemDTO> Items { get; set; }

        public CreateOrderDTO()
        {
            Items = new List<OrderItemDTO>();
        }
    }

    /// <summary>
    /// DTO cho item trong đơn hàng
    /// </summary>
    public class OrderItemDTO
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public string Size { get; set; }
        public string SugarLevel { get; set; }
        public string IceLevel { get; set; }
        public List<int> ToppingIds { get; set; }

        public OrderItemDTO()
        {
            ToppingIds = new List<int>();
        }
    }
}
