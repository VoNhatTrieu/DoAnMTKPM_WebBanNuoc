using apii.Models.DTOs;
using apii.Models.Entities;
using apii.Repositories;

namespace apii.Services;

/// <summary>
/// Interface cho Order Service
/// </summary>
public interface IOrderService
{
    Task<ApiResponse<OrderDto>> CreateOrderAsync(CreateOrderDto dto, int? userId);
    Task<ApiResponse<OrderDto>> GetOrderByIdAsync(int orderId);
    Task<ApiResponse<List<OrderDto>>> GetOrdersByUserIdAsync(int userId);
    Task<ApiResponse<string>> UpdateOrderStatusAsync(int orderId, string status);
}

/// <summary>
/// Order Service
/// </summary>
public class OrderService : IOrderService
{
    private readonly IUnitOfWork _unitOfWork;

    public OrderService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ApiResponse<OrderDto>> CreateOrderAsync(CreateOrderDto dto, int? userId)
    {
        try
        {
            await _unitOfWork.BeginTransactionAsync();

            // Validate
            if (dto.Items == null || !dto.Items.Any())
            {
                return ApiResponse<OrderDto>.ErrorResponse("Đơn hàng phải có ít nhất 1 sản phẩm");
            }

            // Calculate totals
            decimal subtotal = 0;
            var orderDetails = new List<OrderDetail>();

            foreach (var item in dto.Items)
            {
                var product = await _unitOfWork.Repository<Product>().GetByIdAsync(item.ProductId);
                if (product == null)
                {
                    return ApiResponse<OrderDto>.ErrorResponse($"Không tìm thấy sản phẩm ID {item.ProductId}");
                }

                var totalPrice = product.BasePrice * item.Quantity;
                subtotal += totalPrice;

                orderDetails.Add(new OrderDetail
                {
                    ProductId = product.Id,
                    ProductName = product.Name,
                    ImageUrl = product.ImageUrl,
                    UnitPrice = product.BasePrice,
                    Quantity = item.Quantity,
                    Size = item.Size,
                    SugarLevel = item.SugarLevel,
                    IceLevel = item.IceLevel,
                    Toppings = item.Toppings,
                    TotalPrice = totalPrice
                });
            }

            var shippingFee = CalculateShippingFee(subtotal);
            var discount = 0m;
            var total = subtotal + shippingFee - discount;

            // Create order
            var order = new Order
            {
                OrderNumber = GenerateOrderNumber(),
                UserId = userId,
                OrderDate = DateTime.Now,
                Status = "Pending",
                PaymentMethod = dto.PaymentMethod,
                PaymentStatus = "Pending",
                CustomerName = dto.CustomerName,
                CustomerPhone = dto.CustomerPhone,
                CustomerEmail = dto.CustomerEmail,
                ShippingAddress = dto.ShippingAddress,
                Notes = dto.Notes,
                Subtotal = subtotal,
                ShippingFee = shippingFee,
                Discount = discount,
                Total = total
            };

            await _unitOfWork.Repository<Order>().AddAsync(order);
            await _unitOfWork.SaveChangesAsync();

            // Add order details
            foreach (var detail in orderDetails)
            {
                detail.OrderId = order.Id;
            }
            await _unitOfWork.Repository<OrderDetail>().AddRangeAsync(orderDetails);
            await _unitOfWork.SaveChangesAsync();

            await _unitOfWork.CommitTransactionAsync();

            var orderDto = MapToOrderDto(order, orderDetails);
            return ApiResponse<OrderDto>.SuccessResponse(orderDto, "Đặt hàng thành công");
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackTransactionAsync();
            return ApiResponse<OrderDto>.ErrorResponse($"Lỗi: {ex.Message}");
        }
    }

    public async Task<ApiResponse<OrderDto>> GetOrderByIdAsync(int orderId)
    {
        try
        {
            var order = await _unitOfWork.Repository<Order>().GetByIdAsync(orderId);
            if (order == null)
            {
                return ApiResponse<OrderDto>.ErrorResponse("Không tìm thấy đơn hàng");
            }

            var orderDetails = await _unitOfWork.Repository<OrderDetail>()
                .FindAsync(od => od.OrderId == orderId);

            var orderDto = MapToOrderDto(order, orderDetails.ToList());
            return ApiResponse<OrderDto>.SuccessResponse(orderDto, "Lấy thông tin đơn hàng thành công");
        }
        catch (Exception ex)
        {
            return ApiResponse<OrderDto>.ErrorResponse($"Lỗi: {ex.Message}");
        }
    }

    public async Task<ApiResponse<List<OrderDto>>> GetOrdersByUserIdAsync(int userId)
    {
        try
        {
            var orders = await _unitOfWork.Repository<Order>()
                .FindAsync(o => o.UserId == userId);

            var orderDtos = new List<OrderDto>();
            foreach (var order in orders.OrderByDescending(o => o.OrderDate))
            {
                var orderDetails = await _unitOfWork.Repository<OrderDetail>()
                    .FindAsync(od => od.OrderId == order.Id);
                orderDtos.Add(MapToOrderDto(order, orderDetails.ToList()));
            }

            return ApiResponse<List<OrderDto>>.SuccessResponse(orderDtos, "Lấy danh sách đơn hàng thành công");
        }
        catch (Exception ex)
        {
            return ApiResponse<List<OrderDto>>.ErrorResponse($"Lỗi: {ex.Message}");
        }
    }

    public async Task<ApiResponse<string>> UpdateOrderStatusAsync(int orderId, string status)
    {
        try
        {
            var order = await _unitOfWork.Repository<Order>().GetByIdAsync(orderId);
            if (order == null)
            {
                return ApiResponse<string>.ErrorResponse("Không tìm thấy đơn hàng");
            }

            order.Status = status;
            _unitOfWork.Repository<Order>().Update(order);
            await _unitOfWork.SaveChangesAsync();

            return ApiResponse<string>.SuccessResponse("OK", "Cập nhật trạng thái đơn hàng thành công");
        }
        catch (Exception ex)
        {
            return ApiResponse<string>.ErrorResponse($"Lỗi: {ex.Message}");
        }
    }

    private OrderDto MapToOrderDto(Order order, List<OrderDetail> orderDetails)
    {
        return new OrderDto
        {
            Id = order.Id,
            OrderNumber = order.OrderNumber,
            OrderDate = order.OrderDate,
            Status = order.Status,
            PaymentMethod = order.PaymentMethod ?? "COD",
            PaymentStatus = order.PaymentStatus,
            CustomerName = order.CustomerName,
            CustomerPhone = order.CustomerPhone,
            CustomerEmail = order.CustomerEmail ?? string.Empty,
            ShippingAddress = order.ShippingAddress ?? string.Empty,
            Notes = order.Notes ?? string.Empty,
            Subtotal = order.Subtotal,
            ShippingFee = order.ShippingFee,
            Discount = order.Discount,
            Total = order.Total,
            OrderDetails = orderDetails.Select(od => new OrderDetailDto
            {
                ProductId = od.ProductId,
                ProductName = od.ProductName,
                ImageUrl = od.ImageUrl ?? string.Empty,
                UnitPrice = od.UnitPrice,
                Quantity = od.Quantity,
                Size = od.Size ?? string.Empty,
                SugarLevel = od.SugarLevel,
                IceLevel = od.IceLevel,
                Toppings = od.Toppings ?? string.Empty,
                TotalPrice = od.TotalPrice
            }).ToList()
        };
    }

    private string GenerateOrderNumber()
    {
        return $"ORD{DateTime.Now:yyyyMMddHHmmss}{new Random().Next(100, 999)}";
    }

    private decimal CalculateShippingFee(decimal subtotal)
    {
        return subtotal >= 100000 ? 0 : 20000;
    }
}
