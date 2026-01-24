using Microsoft.AspNetCore.Mvc;
using apii.Models.DTOs;
using apii.Services;

namespace apii.Controllers;

/// <summary>
/// Controller cho quản lý danh mục (Admin)
/// </summary>
[ApiController]
[Route("api/admin/categories")]
public class AdminCategoriesController : ControllerBase
{
    private readonly IAdminService _adminService;

    public AdminCategoriesController(IAdminService adminService)
    {
        _adminService = adminService;
    }

    /// <summary>
    /// GET: Lấy tất cả danh mục
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<ApiResponse<List<AdminCategoryDto>>>> GetAllCategories()
    {
        try
        {
            var categories = await _adminService.GetAllCategoriesAsync();
            return Ok(new ApiResponse<List<AdminCategoryDto>>
            {
                Success = true,
                Message = "Lấy danh sách danh mục thành công",
                Data = categories
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new ApiResponse<List<AdminCategoryDto>>
            {
                Success = false,
                Message = $"Lỗi: {ex.Message}"
            });
        }
    }

    /// <summary>
    /// GET: Lấy chi tiết danh mục theo ID
    /// </summary>
    [HttpGet("{categoryId}")]
    public async Task<ActionResult<ApiResponse<AdminCategoryDto>>> GetCategoryById(int categoryId)
    {
        try
        {
            var category = await _adminService.GetCategoryByIdAsync(categoryId);
            if (category == null)
            {
                return NotFound(new ApiResponse<AdminCategoryDto>
                {
                    Success = false,
                    Message = "Không tìm thấy danh mục"
                });
            }

            return Ok(new ApiResponse<AdminCategoryDto>
            {
                Success = true,
                Message = "Lấy thông tin danh mục thành công",
                Data = category
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new ApiResponse<AdminCategoryDto>
            {
                Success = false,
                Message = $"Lỗi: {ex.Message}"
            });
        }
    }

    /// <summary>
    /// POST: Tạo danh mục mới
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<ApiResponse<object>>> CreateCategory([FromBody] CreateCategoryDto dto)
    {
        try
        {
            var category = await _adminService.CreateCategoryAsync(dto);
            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "Tạo danh mục thành công",
                Data = new { category.Id, category.Name }
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
    /// PUT: Cập nhật danh mục
    /// </summary>
    [HttpPut("{categoryId}")]
    public async Task<ActionResult<ApiResponse<object>>> UpdateCategory(int categoryId, [FromBody] UpdateCategoryDto dto)
    {
        try
        {
            var category = await _adminService.UpdateCategoryAsync(categoryId, dto);
            if (category == null)
            {
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Không tìm thấy danh mục"
                });
            }

            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "Cập nhật danh mục thành công",
                Data = new { category.Id, category.Name }
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
    /// DELETE: Xóa danh mục
    /// </summary>
    [HttpDelete("{categoryId}")]
    public async Task<ActionResult<ApiResponse<string>>> DeleteCategory(int categoryId)
    {
        try
        {
            var success = await _adminService.DeleteCategoryAsync(categoryId);
            if (!success)
            {
                return NotFound(new ApiResponse<string>
                {
                    Success = false,
                    Message = "Không tìm thấy danh mục hoặc danh mục đang có sản phẩm"
                });
            }

            return Ok(new ApiResponse<string>
            {
                Success = true,
                Message = "Xóa danh mục thành công"
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
