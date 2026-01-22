using Microsoft.AspNetCore.Mvc;
using apii.Models.DTOs;
using apii.Services;

namespace apii.Controllers;

/// <summary>
/// Controller cho Cart (Giỏ hàng)
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class CartController : ControllerBase
{
    private readonly ICartService _cartService;

    public CartController(ICartService cartService)
    {
        _cartService = cartService;
    }

    /// <summary>
    /// Lấy giỏ hàng
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<ApiResponse<CartSummaryDto>>> GetCart([FromQuery] int? userId, [FromQuery] string? sessionId)
    {
        var result = await _cartService.GetCartAsync(userId, sessionId);
        return Ok(result);
    }

    /// <summary>
    /// Thêm sản phẩm vào giỏ hàng
    /// </summary>
    [HttpPost("add")]
    public async Task<ActionResult<ApiResponse<string>>> AddToCart(
        [FromBody] AddToCartDto dto, 
        [FromQuery] int? userId, 
        [FromQuery] string? sessionId)
    {
        var result = await _cartService.AddToCartAsync(dto, userId, sessionId);
        
        if (!result.Success)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }

    /// <summary>
    /// Cập nhật số lượng sản phẩm trong giỏ hàng
    /// </summary>
    [HttpPost("update/{cartItemId}")]
    public async Task<ActionResult<ApiResponse<string>>> UpdateCartItem(int cartItemId, [FromBody] int quantity)
    {
        var result = await _cartService.UpdateCartItemAsync(cartItemId, quantity);
        
        if (!result.Success)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }

    /// <summary>
    /// Xóa sản phẩm khỏi giỏ hàng
    /// </summary>
    [HttpPost("remove/{cartItemId}")]
    public async Task<ActionResult<ApiResponse<string>>> RemoveCartItem(int cartItemId)
    {
        var result = await _cartService.RemoveCartItemAsync(cartItemId);
        
        if (!result.Success)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }

    /// <summary>
    /// Xóa toàn bộ giỏ hàng
    /// </summary>
    [HttpPost("clear")]
    public async Task<ActionResult<ApiResponse<string>>> ClearCart([FromQuery] int? userId, [FromQuery] string? sessionId)
    {
        var result = await _cartService.ClearCartAsync(userId, sessionId);
        return Ok(result);
    }
}
