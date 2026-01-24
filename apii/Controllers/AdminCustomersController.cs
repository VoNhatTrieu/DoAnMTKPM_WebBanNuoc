using Microsoft.AspNetCore.Mvc;
using apii.Models.DTOs;
using apii.Services;

namespace apii.Controllers;

/// <summary>
/// Controller cho quản lý khách hàng (Admin)
/// </summary>
[ApiController]
[Route("api/admin/customers")]
public class AdminCustomersController : ControllerBase
{
    private readonly IAdminService _adminService;

    public AdminCustomersController(IAdminService adminService)
    {
        _adminService = adminService;
    }

    /// <summary>
    /// GET: Lấy tất cả khách hàng
    /// </summary>
    [HttpGet]
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
    /// GET: Lấy chi tiết khách hàng theo ID
    /// </summary>
    [HttpGet("{customerId}")]
    public async Task<ActionResult<ApiResponse<CustomerDetailDto>>> GetCustomerDetail(int customerId)
    {
        try
        {
            var customer = await _adminService.GetCustomerDetailAsync(customerId);
            if (customer == null)
            {
                return NotFound(new ApiResponse<CustomerDetailDto>
                {
                    Success = false,
                    Message = "Không tìm thấy khách hàng"
                });
            }

            return Ok(new ApiResponse<CustomerDetailDto>
            {
                Success = true,
                Message = "Lấy thông tin khách hàng thành công",
                Data = customer
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new ApiResponse<CustomerDetailDto>
            {
                Success = false,
                Message = $"Lỗi: {ex.Message}"
            });
        }
    }

    /// <summary>
    /// POST: Tạo khách hàng mới
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<ApiResponse<object>>> CreateCustomer([FromBody] CreateCustomerDto dto)
    {
        try
        {
            var customer = await _adminService.CreateCustomerAsync(dto);
            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "Tạo khách hàng thành công",
                Data = new { customer.Id, customer.FullName, customer.Email }
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
    /// PUT: Cập nhật thông tin khách hàng
    /// </summary>
    [HttpPut("{customerId}")]
    public async Task<ActionResult<ApiResponse<object>>> UpdateCustomer(int customerId, [FromBody] UpdateCustomerDto dto)
    {
        try
        {
            var customer = await _adminService.UpdateCustomerAsync(customerId, dto);
            if (customer == null)
            {
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Không tìm thấy khách hàng"
                });
            }

            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "Cập nhật thông tin khách hàng thành công",
                Data = new { customer.Id, customer.FullName }
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
    /// DELETE: Xóa khách hàng (soft delete)
    /// </summary>
    [HttpDelete("{customerId}")]
    public async Task<ActionResult<ApiResponse<string>>> DeleteCustomer(int customerId)
    {
        try
        {
            var success = await _adminService.DeleteCustomerAsync(customerId);
            if (!success)
            {
                return NotFound(new ApiResponse<string>
                {
                    Success = false,
                    Message = "Không tìm thấy khách hàng"
                });
            }

            return Ok(new ApiResponse<string>
            {
                Success = true,
                Message = "Xóa khách hàng thành công"
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
