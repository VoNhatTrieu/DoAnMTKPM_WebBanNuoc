using Microsoft.AspNetCore.Mvc;
using apii.Models.DTOs;
using apii.Services;

namespace apii.Controllers;

/// <summary>
/// Controller cho Admin - Dashboard và quản lý (Using Service Pattern)
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class AdminController : ControllerBase
{
    private readonly IAdminService _adminService;

    public AdminController(IAdminService adminService)
    {
        _adminService = adminService;
    }

    /// <summary>
    /// Lấy thống kê dashboard
    /// </summary>
    [HttpGet("dashboard/stats")]
    public async Task<ActionResult<ApiResponse<DashboardStatsDto>>> GetDashboardStats([FromQuery] string period = "today")
    {
        try
        {
            var stats = await _adminService.GetDashboardStatsAsync(period);
            return Ok(new ApiResponse<DashboardStatsDto>
            {
                Success = true,
                Message = "Lấy thống kê thành công",
                Data = stats
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new ApiResponse<DashboardStatsDto>
            {
                Success = false,
                Message = $"Lỗi: {ex.Message}"
            });
        }
    }

    /// <summary>
    /// Lấy doanh thu theo ngày (7 ngày gần nhất)
    /// </summary>
    [HttpGet("dashboard/revenue-chart")]
    public async Task<ActionResult<ApiResponse<List<RevenueChartDto>>>> GetRevenueChart()
    {
        try
        {
            var revenueData = await _adminService.GetRevenueChartAsync();
            return Ok(new ApiResponse<List<RevenueChartDto>>
            {
                Success = true,
                Message = "Lấy dữ liệu biểu đồ thành công",
                Data = revenueData
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new ApiResponse<List<RevenueChartDto>>
            {
                Success = false,
                Message = $"Lỗi: {ex.Message}"
            });
        }
    }

    /// <summary>
    /// Lấy top sản phẩm bán chạy
    /// </summary>
    [HttpGet("dashboard/top-products")]
    public async Task<ActionResult<ApiResponse<List<TopProductDto>>>> GetTopProducts([FromQuery] int limit = 5)
    {
        try
        {
            var topProducts = await _adminService.GetTopProductsAsync(limit);
            return Ok(new ApiResponse<List<TopProductDto>>
            {
                Success = true,
                Message = "Lấy top sản phẩm thành công",
                Data = topProducts
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new ApiResponse<List<TopProductDto>>
            {
                Success = false,
                Message = $"Lỗi: {ex.Message}"
            });
        }
    }

    /// <summary>
    /// Lấy danh sách đơn hàng gần đây
    /// </summary>
    [HttpGet("orders/recent")]
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
    /// Lấy tất cả đơn hàng (với phân trang)
    /// </summary>
    [HttpGet("orders")]
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
    /// Lấy chi tiết đơn hàng
    /// </summary>
    [HttpGet("orders/{orderId}")]
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
    /// Cập nhật trạng thái đơn hàng
    /// </summary>
    [HttpPut("orders/{orderId}/status")]
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
    /// Lấy danh sách khách hàng
    /// </summary>
    [HttpGet("customers")]
    public async Task<ActionResult<ApiResponse<List<CustomerDto>>>> GetAllCustomers()
    {
        try
        {
            var customers = await _adminService.GetAllCustomersAsync();
            return Ok(new ApiResponse<List<CustomerDto>>
            {
                Success = true,
                Message = "Lấy danh sách khách hàng thành công",
                Data = customers
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new ApiResponse<List<CustomerDto>>
            {
                Success = false,
                Message = $"Lỗi: {ex.Message}"
            });
        }
    }

    /// <summary>
    /// Lấy tất cả sản phẩm cho admin (bao gồm cả sản phẩm inactive)
    /// </summary>
    [HttpGet("products")]
    public async Task<ActionResult<ApiResponse<List<AdminProductDto>>>> GetAllProductsForAdmin()
    {
        try
        {
            var products = await _adminService.GetAllProductsAsync();
            return Ok(new ApiResponse<List<AdminProductDto>>
            {
                Success = true,
                Message = "Lấy danh sách sản phẩm thành công",
                Data = products
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new ApiResponse<List<AdminProductDto>>
            {
                Success = false,
                Message = $"Lỗi: {ex.Message}"
            });
        }
    }

    /// <summary>
    /// Lấy chi tiết sản phẩm theo ID
    /// </summary>
    [HttpGet("products/{id}")]
    public async Task<ActionResult<ApiResponse<AdminProductDto>>> GetProductById(int id)
    {
        try
        {
            var product = await _adminService.GetProductByIdAsync(id);
            if (product == null)
            {
                return NotFound(new ApiResponse<AdminProductDto>
                {
                    Success = false,
                    Message = "Không tìm thấy sản phẩm"
                });
            }

            return Ok(new ApiResponse<AdminProductDto>
            {
                Success = true,
                Message = "Lấy thông tin sản phẩm thành công",
                Data = product
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new ApiResponse<AdminProductDto>
            {
                Success = false,
                Message = $"Lỗi: {ex.Message}"
            });
        }
    }

    /// <summary>
    /// Tạo sản phẩm mới
    /// </summary>
    [HttpPost("products")]
    public async Task<ActionResult<ApiResponse<object>>> CreateProduct([FromBody] CreateProductDto dto)
    {
        try
        {
            var product = await _adminService.CreateProductAsync(dto);
            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "Tạo sản phẩm thành công",
                Data = new { product.Id, product.Name }
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
    /// Cập nhật sản phẩm
    /// </summary>
    [HttpPut("products/{productId}")]
    public async Task<ActionResult<ApiResponse<object>>> UpdateProduct(int productId, [FromBody] UpdateProductDto dto)
    {
        try
        {
            var product = await _adminService.UpdateProductAsync(productId, dto);
            if (product == null)
            {
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Không tìm thấy sản phẩm"
                });
            }

            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "Cập nhật sản phẩm thành công",
                Data = new { product.Id, product.Name }
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
    /// Xóa sản phẩm (soft delete)
    /// </summary>
    [HttpDelete("products/{productId}")]
    public async Task<ActionResult<ApiResponse<string>>> DeleteProduct(int productId)
    {
        try
        {
            var success = await _adminService.DeleteProductAsync(productId);
            if (!success)
            {
                return NotFound(new ApiResponse<string>
                {
                    Success = false,
                    Message = "Không tìm thấy sản phẩm"
                });
            }

            return Ok(new ApiResponse<string>
            {
                Success = true,
                Message = "Xóa sản phẩm thành công"
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

    // POST: api/admin/seed-products
    [HttpPost("seed-products")]
    public async Task<IActionResult> SeedProducts([FromServices] SeedDataService seedService)
    {
        var count = await seedService.SeedProductsAsync();
        if (count == 0)
        {
            return Ok(new { message = "Sản phẩm đã tồn tại, không cần seed", productsAdded = 0 });
        }
        return Ok(new { message = $"Đã thêm thành công {count} sản phẩm", productsAdded = count });
    }
}
