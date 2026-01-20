using System;
using System.Collections.Generic;
using System.Linq;
using WebBanNuoc.Models.DTOs;
using WebBanNuoc.Models.Entities;

namespace WebBanNuoc.Services.Implementations
{
    /// <summary>
    /// Local Order Service với dữ liệu tĩnh
    /// Thay thế API calls bằng dữ liệu mẫu
    /// </summary>
    public class LocalOrderService : IOrderService
    {
        private static List<Order> _orders = new List<Order>();
        private static int _nextOrderId = 1;

        public Order CreateOrder(CreateOrderDTO orderDto)
        {
            if (!ValidateOrder(orderDto))
            {
                throw new ArgumentException("Invalid order data");
            }

            var order = new Order
            {
                Id = _nextOrderId++,
                OrderNumber = GenerateOrderNumber(),
                CustomerName = orderDto.CustomerName,
                CustomerPhone = orderDto.CustomerPhone,
                CustomerEmail = orderDto.CustomerEmail,
                ShippingAddress = orderDto.ShippingAddress,
                Notes = orderDto.Notes,
                OrderDate = DateTime.Now,
                Status = OrderStatus.Pending,
                PaymentMethod = ParsePaymentMethod(orderDto.PaymentMethod),
                PaymentStatus = PaymentStatus.Pending,
                OrderDetails = new List<OrderDetail>()
            };

            // Tạo order details từ items
            decimal subtotal = 0;
            foreach (var item in orderDto.Items)
            {
                var orderDetail = new OrderDetail
                {
                    Id = order.OrderDetails.Count + 1,
                    OrderId = order.Id,
                    ProductId = item.ProductId,
                    ProductName = GetProductName(item.ProductId),
                    ImageUrl = GetProductImage(item.ProductId),
                    Quantity = item.Quantity,
                    Size = item.Size,
                    SugarLevel = item.SugarLevel,
                    IceLevel = item.IceLevel,
                    Toppings = string.Join(", ", item.ToppingIds.Select(t => GetToppingName(t))),
                    UnitPrice = CalculateItemPrice(item),
                    TotalPrice = CalculateItemPrice(item) * item.Quantity
                };

                order.OrderDetails.Add(orderDetail);
                subtotal += orderDetail.TotalPrice;
            }

            order.Subtotal = subtotal;
            order.ShippingFee = CalculateShippingFee(subtotal);
            order.Discount = 0; // Có thể áp dụng voucher sau
            order.Total = order.Subtotal + order.ShippingFee - order.Discount;

            _orders.Add(order);
            return order;
        }

        public Order GetOrderById(int id)
        {
            return _orders.FirstOrDefault(o => o.Id == id);
        }

        public IEnumerable<Order> GetAllOrders()
        {
            return _orders.OrderByDescending(o => o.OrderDate).ToList();
        }

        public void UpdateOrderStatus(int orderId, OrderStatus status)
        {
            var order = _orders.FirstOrDefault(o => o.Id == orderId);
            if (order != null)
            {
                order.Status = status;
            }
        }

        public decimal CalculateOrderTotal(List<CartItem> items)
        {
            return items.Sum(item => item.UnitPrice * item.Quantity);
        }

        public bool ValidateOrder(CreateOrderDTO orderDto)
        {
            if (string.IsNullOrEmpty(orderDto.CustomerName) ||
                string.IsNullOrEmpty(orderDto.CustomerPhone) ||
                string.IsNullOrEmpty(orderDto.ShippingAddress))
            {
                return false;
            }

            if (orderDto.Items == null || !orderDto.Items.Any())
            {
                return false;
            }

            return true;
        }

        private string GenerateOrderNumber()
        {
            return $"ORD{DateTime.Now:yyyyMMddHHmmss}";
        }

        private PaymentMethod ParsePaymentMethod(string method)
        {
            switch (method?.ToUpper())
            {
                case "COD":
                    return PaymentMethod.COD;
                case "MOMO":
                    return PaymentMethod.MoMo;
                case "BANKING":
                    return PaymentMethod.Banking;
                case "VNPAY":
                    return PaymentMethod.VNPay;
                default:
                    return PaymentMethod.COD;
            }
        }

        private string GetProductName(int productId)
        {
            // Danh sách sản phẩm đơn giản
            var products = new Dictionary<int, string>
            {
                { 1, "Nước khoáng Lavie 500ml" },
                { 2, "Nước khoáng Aquafina 500ml" },
                { 3, "Nước khoáng Dasani 500ml" },
                { 4, "Nước suối Vĩnh Hảo 500ml" },
                { 5, "Nước tinh khiết Miru 500ml" },
                { 6, "Nước khoáng Evian 500ml" },
                { 7, "Nước suối Lavie 1.5L" },
                { 8, "Nước khoáng Aquafina 1.5L" },
                { 9, "Nước có ga Sprite 330ml" },
                { 10, "Nước có ga Coca-Cola 330ml" },
                { 11, "Nước có ga Pepsi 330ml" },
                { 12, "Nước ép cam Minute Maid 1L" }
            };

            return products.ContainsKey(productId) ? products[productId] : "Unknown Product";
        }

        private string GetProductImage(int productId)
        {
            var images = new Dictionary<int, string>
            {
                { 1, "/Content/images/lavie-500ml.jpg" },
                { 2, "/Content/images/aquafina-500ml.jpg" },
                { 3, "/Content/images/dasani-500ml.jpg" },
                { 4, "/Content/images/vinhhao-500ml.jpg" },
                { 5, "/Content/images/miru-500ml.jpg" },
                { 6, "/Content/images/evian-500ml.jpg" },
                { 7, "/Content/images/lavie-1500ml.jpg" },
                { 8, "/Content/images/aquafina-1500ml.jpg" },
                { 9, "/Content/images/sprite-330ml.jpg" },
                { 10, "/Content/images/coca-330ml.jpg" },
                { 11, "/Content/images/pepsi-330ml.jpg" },
                { 12, "/Content/images/minutemaid-1000ml.jpg" }
            };

            return images.ContainsKey(productId) ? images[productId] : "/Content/images/default.jpg";
        }

        private string GetToppingName(int toppingId)
        {
            var toppings = new Dictionary<int, string>
            {
                { 1, "Trân châu" },
                { 2, "Thạch" },
                { 3, "Pudding" },
                { 4, "Trân châu hoàng kim" }
            };

            return toppings.ContainsKey(toppingId) ? toppings[toppingId] : "";
        }

        private decimal CalculateItemPrice(OrderItemDTO item)
        {
            // Giá cơ bản
            var basePrice = GetProductBasePrice(item.ProductId);

            // Cộng giá size
            decimal sizePrice = 0;
            switch (item.Size?.ToUpper())
            {
                case "M":
                    sizePrice = 5000;
                    break;
                case "L":
                    sizePrice = 10000;
                    break;
            }

            // Cộng giá topping
            decimal toppingPrice = 0;
            foreach (var toppingId in item.ToppingIds ?? new List<int>())
            {
                toppingPrice += GetToppingPrice(toppingId);
            }

            return basePrice + sizePrice + toppingPrice;
        }

        private decimal GetProductBasePrice(int productId)
        {
            var prices = new Dictionary<int, decimal>
            {
                { 1, 5000 },
                { 2, 5000 },
                { 3, 6000 },
                { 4, 4500 },
                { 5, 4000 },
                { 6, 35000 },
                { 7, 10000 },
                { 8, 10000 },
                { 9, 8000 },
                { 10, 8000 },
                { 11, 8000 },
                { 12, 25000 }
            };

            return prices.ContainsKey(productId) ? prices[productId] : 5000;
        }

        private decimal GetToppingPrice(int toppingId)
        {
            var prices = new Dictionary<int, decimal>
            {
                { 1, 5000 },
                { 2, 5000 },
                { 3, 7000 },
                { 4, 8000 }
            };

            return prices.ContainsKey(toppingId) ? prices[toppingId] : 0;
        }

        private decimal CalculateShippingFee(decimal subtotal)
        {
            // Miễn phí ship nếu đơn hàng trên 100,000đ
            if (subtotal >= 100000)
                return 0;

            // Phí ship cố định 20,000đ
            return 20000;
        }
    }
}
