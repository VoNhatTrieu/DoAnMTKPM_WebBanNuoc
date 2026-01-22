using Microsoft.AspNetCore.Mvc;
using apii.Models.DTOs;
using apii.Services;

namespace apii.Controllers;

/// <summary>
/// Controller cho Orders (Đơn hàng)
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class OrdersController : ControllerBase
{
    private readonly IOrderService _orderService;

    public OrdersController(IOrderService orderService)
    {
        _orderService = orderService;
    }

    /// <summary>
    /// Tạo đơn hàng mới
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<ApiResponse<OrderDto>>> CreateOrder(
        [FromBody] CreateOrderDto dto, 
        [FromQuery] int? userId)
    {
        var result = await _orderService.CreateOrderAsync(dto, userId);
        
        if (!result.Success)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }

    /// <summary>
    /// Lấy thông tin đơn hàng theo ID
    /// </summary>
    [HttpGet("{orderId}")]
    public async Task<ActionResult<ApiResponse<OrderDto>>> GetOrderById(int orderId)
    {
        var result = await _orderService.GetOrderByIdAsync(orderId);
        
        if (!result.Success)
        {
            return NotFound(result);
        }

        return Ok(result);
    }

    /// <summary>
    /// Lấy danh sách đơn hàng của user
    /// </summary>
    [HttpGet("user/{userId}")]
    public async Task<ActionResult<ApiResponse<List<OrderDto>>>> GetOrdersByUserId(int userId)
    {
        var result = await _orderService.GetOrdersByUserIdAsync(userId);
        return Ok(result);
    }

    /// <summary>
    /// Cập nhật trạng thái đơn hàng
    /// </summary>
    [HttpPost("{orderId}/status")]
    public async Task<ActionResult<ApiResponse<string>>> UpdateOrderStatus(
        int orderId, 
        [FromBody] string status)
    {
        var result = await _orderService.UpdateOrderStatusAsync(orderId, status);
        
        if (!result.Success)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }
}
