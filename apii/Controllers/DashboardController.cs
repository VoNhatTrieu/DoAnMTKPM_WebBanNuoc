using Microsoft.AspNetCore.Mvc;
using apii.Models.DTOs;
using apii.Services;

namespace apii.Controllers;

/// <summary>
/// Controller cho Dashboard - Thống kê và biểu đồ
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class DashboardController : ControllerBase
{
    private readonly IAdminService _adminService;

    public DashboardController(IAdminService adminService)
    {
        _adminService = adminService;
    }

    /// <summary>
    /// GET: Lấy thống kê dashboard
    /// </summary>
    [HttpGet("stats")]
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
    /// GET: Lấy dữ liệu biểu đồ doanh thu (7 ngày gần nhất)
    /// </summary>
    [HttpGet("revenue-chart")]
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
    /// GET: Lấy top sản phẩm bán chạy
    /// </summary>
    [HttpGet("top-products")]
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
}
