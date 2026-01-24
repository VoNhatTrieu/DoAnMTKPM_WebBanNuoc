using Microsoft.AspNetCore.Mvc;
using apii.Models.DTOs;
using apii.Services;

namespace apii.Controllers;

/// <summary>
/// Controller cho quản lý đơn hàng (Admin)
/// </summary>
[ApiController]
[Route("api/admin/orders")]
public class AdminOrdersController : ControllerBase
{
    private readonly IAdminService _adminService;

    public AdminOrdersController(IAdminService adminService)
    {
        _adminService = adminService;
    }

    /// <summary>
    /// GET: Lấy tất cả đơn hàng (với phân trang và lọc)
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<ApiResponse<PagedResult<AdminOrderDto>>>> GetAllOrders(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? status = null)
    {
        try
        {
            var result = await _adminService.GetAllOrdersAsync(page, pageSize, status);
            return Ok(new ApiResponse<PagedResult<AdminOrderDto>>
            {
                Success = true,
                Message = "Lấy danh sách đơn hàng thành công",
                Data = result
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new ApiResponse<PagedResult<AdminOrderDto>>
            {
                Success = false,
                Message = $"Lỗi: {ex.Message}"
            });
        }
    }

    /// <summary>
    /// GET: Lấy chi tiết đơn hàng theo ID
    /// </summary>
    [HttpGet("{orderId}")]
    public async Task<ActionResult<ApiResponse<AdminOrderDetailDto>>> GetOrderDetail(int orderId)
    {
        try
        {
            var orderDetail = await _adminService.GetOrderDetailAsync(orderId);
            if (orderDetail == null)
            {
                return NotFound(new ApiResponse<AdminOrderDetailDto>
                {
                    Success = false,
                    Message = "Không tìm thấy đơn hàng"
                });
            }

            return Ok(new ApiResponse<AdminOrderDetailDto>
            {
                Success = true,
                Message = "Lấy chi tiết đơn hàng thành công",
                Data = orderDetail
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new ApiResponse<AdminOrderDetailDto>
            {
                Success = false,
                Message = $"Lỗi: {ex.Message}"
            });
        }
    }

    /// <summary>
    /// GET: Lấy danh sách đơn hàng gần đây
    /// </summary>
    [HttpGet("recent")]
    public async Task<ActionResult<ApiResponse<List<AdminOrderDto>>>> GetRecentOrders([FromQuery] int limit = 10)
    {
        try
        {
            var orders = await _adminService.GetRecentOrdersAsync(limit);
            return Ok(new ApiResponse<List<AdminOrderDto>>
            {
                Success = true,
                Message = "Lấy đơn hàng gần đây thành công",
                Data = orders
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new ApiResponse<List<AdminOrderDto>>
            {
                Success = false,
                Message = $"Lỗi: {ex.Message}"
            });
        }
    }

    /// <summary>
    /// POST: Tạo đơn hàng mới (Admin tạo cho khách)
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<ApiResponse<object>>> CreateOrder([FromBody] AdminCreateOrderDto dto)
    {
        try
        {
            var order = await _adminService.CreateOrderAsync(dto);
            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "Tạo đơn hàng thành công",
                Data = new { order.Id, order.OrderDate }
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new ApiResponse<object>
            {
                Success = false,
                Message = $"Lỗi: {ex.Message}"
            });
        }
    }

    /// <summary>
    /// PUT: Cập nhật trạng thái đơn hàng
    /// </summary>
    [HttpPut("{orderId}/status")]
    public async Task<ActionResult<ApiResponse<string>>> UpdateOrderStatus(int orderId, [FromBody] UpdateOrderStatusDto dto)
    {
        try
        {
            var success = await _adminService.UpdateOrderStatusAsync(orderId, dto.Status);
            if (!success)
            {
                return NotFound(new ApiResponse<string>
                {
                    Success = false,
                    Message = "Không tìm thấy đơn hàng"
                });
            }

            return Ok(new ApiResponse<string>
            {
                Success = true,
                Message = "Cập nhật trạng thái thành công",
                Data = dto.Status
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new ApiResponse<string>
            {
                Success = false,
                Message = $"Lỗi: {ex.Message}"
            });
        }
    }

    /// <summary>
    /// PUT: Cập nhật thông tin đơn hàng
    /// </summary>
    [HttpPut("{orderId}")]
    public async Task<ActionResult<ApiResponse<object>>> UpdateOrder(int orderId, [FromBody] UpdateOrderDto dto)
    {
        try
        {
            var order = await _adminService.UpdateOrderAsync(orderId, dto);
            if (order == null)
            {
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Không tìm thấy đơn hàng"
                });
            }

            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "Cập nhật đơn hàng thành công",
                Data = new { order.Id, order.Status }
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new ApiResponse<object>
            {
                Success = false,
                Message = $"Lỗi: {ex.Message}"
            });
        }
    }

    /// <summary>
    /// DELETE: Xóa đơn hàng
    /// </summary>
    [HttpDelete("{orderId}")]
    public async Task<ActionResult<ApiResponse<string>>> DeleteOrder(int orderId)
    {
        try
        {
            var success = await _adminService.DeleteOrderAsync(orderId);
            if (!success)
            {
                return NotFound(new ApiResponse<string>
                {
                    Success = false,
                    Message = "Không tìm thấy đơn hàng"
                });
            }

            return Ok(new ApiResponse<string>
            {
                Success = true,
                Message = "Xóa đơn hàng thành công"
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new ApiResponse<string>
            {
                Success = false,
                Message = $"Lỗi: {ex.Message}"
            });
        }
    }
}
