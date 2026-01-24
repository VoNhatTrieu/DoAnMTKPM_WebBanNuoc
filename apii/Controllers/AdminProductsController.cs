using Microsoft.AspNetCore.Mvc;
using apii.Models.DTOs;
using apii.Services;

namespace apii.Controllers;

/// <summary>
/// Controller cho quản lý sản phẩm (Admin)
/// </summary>
[ApiController]
[Route("api/admin/products")]
public class AdminProductsController : ControllerBase
{
    private readonly IAdminService _adminService;

    public AdminProductsController(IAdminService adminService)
    {
        _adminService = adminService;
    }

    /// <summary>
    /// GET: Lấy tất cả sản phẩm (bao gồm cả sản phẩm inactive)
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<ApiResponse<List<AdminProductDto>>>> GetAllProducts()
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
    /// GET: Lấy chi tiết sản phẩm theo ID
    /// </summary>
    [HttpGet("{productId}")]
    public async Task<ActionResult<ApiResponse<AdminProductDto>>> GetProductById(int productId)
    {
        try
        {
            var product = await _adminService.GetProductByIdAsync(productId);
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
    /// POST: Tạo sản phẩm mới
    /// </summary>
    [HttpPost]
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
    /// PUT: Cập nhật sản phẩm
    /// </summary>
    [HttpPut("{productId}")]
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
    /// DELETE: Xóa sản phẩm (soft delete)
    /// </summary>
    [HttpDelete("{productId}")]
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
}
