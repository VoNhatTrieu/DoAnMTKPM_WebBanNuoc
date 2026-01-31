using Microsoft.AspNetCore.Mvc;
using apii.Models;
using apii.Models.DTOs;
using apii.Services;

namespace apii.Controllers;

/// <summary>
/// Controller cho quản lý sản phẩm (Admin) với Ownership Validation
/// </summary>
[ApiController]
[Route("api/admin/products")]
public class AdminProductsController : ControllerBase
{
    private readonly IAdminService _adminService;
    private readonly IUserContext _userContext;

    public AdminProductsController(IAdminService adminService, IUserContext userContext)
    {
        _adminService = adminService;
        _userContext = userContext;
    }

    /// <summary>
    /// GET: Lấy tất cả sản phẩm (Admin: tất cả, User: chỉ của mình)
    /// Header: X-User-Id (required), X-User-Role (required)
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<ApiResponse<List<AdminProductDto>>>> GetAllProducts()
    {
        try
        {
            var products = await _adminService.GetAllProductsAsync(_userContext);
            return Ok(new ApiResponse<List<AdminProductDto>>
            {
                Success = true,
                Message = "Lấy danh sách sản phẩm thành công",
                Data = products
            });
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new ApiResponse<List<AdminProductDto>>
            {
                Success = false,
                Message = ex.Message
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
    /// GET: Lấy chi tiết sản phẩm theo ID (với ownership validation)
    /// Header: X-User-Id (required), X-User-Role (required)
    /// </summary>
    [HttpGet("{productId}")]
    public async Task<ActionResult<ApiResponse<AdminProductDto>>> GetProductById(int productId)
    {
        try
        {
            var product = await _adminService.GetProductByIdAsync(productId, _userContext);
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
        catch (OwnershipViolationException ex)
        {
            return Forbid();
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new ApiResponse<AdminProductDto>
            {
                Success = false,
                Message = ex.Message
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
    /// POST: Tạo sản phẩm mới (OwnerId auto-set từ current user)
    /// Header: X-User-Id (required), X-User-Role (required)
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<ApiResponse<object>>> CreateProduct([FromBody] CreateProductDto dto)
    {
        try
        {
            var product = await _adminService.CreateProductAsync(dto, _userContext);
            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "Tạo sản phẩm thành công",
                Data = new { product.Id, product.Name, product.OwnerId }
            });
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new ApiResponse<object>
            {
                Success = false,
                Message = ex.Message
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
    /// PUT: Cập nhật sản phẩm (chỉ owner hoặc admin)
    /// Header: X-User-Id (required), X-User-Role (required)
    /// </summary>
    [HttpPut("{productId}")]
    public async Task<ActionResult<ApiResponse<object>>> UpdateProduct(int productId, [FromBody] UpdateProductDto dto)
    {
        try
        {
            var product = await _adminService.UpdateProductAsync(productId, dto, _userContext);
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
        catch (OwnershipViolationException ex)
        {
            return StatusCode(403, new ApiResponse<object>
            {
                Success = false,
                Message = "Bạn không có quyền cập nhật sản phẩm này"
            });
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new ApiResponse<object>
            {
                Success = false,
                Message = ex.Message
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
    /// DELETE: Xóa sản phẩm (chỉ owner hoặc admin)
    /// Header: X-User-Id (required), X-User-Role (required)
    /// </summary>
    [HttpDelete("{productId}")]
    public async Task<ActionResult<ApiResponse<string>>> DeleteProduct(int productId)
    {
        try
        {
            var success = await _adminService.DeleteProductAsync(productId, _userContext);
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
        catch (OwnershipViolationException ex)
        {
            return StatusCode(403, new ApiResponse<string>
            {
                Success = false,
                Message = "Bạn không có quyền xóa sản phẩm này"
            });
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new ApiResponse<string>
            {
                Success = false,
                Message = ex.Message
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

