using Microsoft.AspNetCore.Mvc;
using apii.Services;

namespace apii.Controllers;

/// <summary>
/// Controller cho việc seed dữ liệu mẫu
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class SeedController : ControllerBase
{
    private readonly SeedDataService _seedService;

    public SeedController(SeedDataService seedService)
    {
        _seedService = seedService;
    }

    /// <summary>
    /// POST: Seed sản phẩm mẫu
    /// </summary>
    [HttpPost("products")]
    public async Task<IActionResult> SeedProducts()
    {
        var count = await _seedService.SeedProductsAsync();
        if (count == 0)
        {
            return Ok(new { message = "Sản phẩm đã tồn tại, không cần seed", productsAdded = 0 });
        }
        return Ok(new { message = $"Đã thêm thành công {count} sản phẩm", productsAdded = count });
    }
}
