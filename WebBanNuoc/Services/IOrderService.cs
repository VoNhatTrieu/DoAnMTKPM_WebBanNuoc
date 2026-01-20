using System.Collections.Generic;
using WebBanNuoc.Models.DTOs;
using WebBanNuoc.Models.Entities;

namespace WebBanNuoc.Services
{
    /// <summary>
    /// Service Pattern - Interface cho Order Service
    /// Đơn giản hóa cho API-based service
    /// </summary>
    public interface IOrderService
    {
        // Order Creation
        Order CreateOrder(CreateOrderDTO orderDto);
        
        // Order Retrieval
        Order GetOrderById(int id);
        IEnumerable<Order> GetAllOrders();
        
        // Order Management
        void UpdateOrderStatus(int orderId, OrderStatus newStatus);
        
        // Validation
        decimal CalculateOrderTotal(List<CartItem> items);
        bool ValidateOrder(CreateOrderDTO orderDto);
    }
}
