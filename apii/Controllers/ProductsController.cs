using Microsoft.AspNetCore.Mvc;
using apii.Models.DTOs;
using apii.Services;

namespace apii.Controllers;

/// <summary>
/// Controller cho Products
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IProductService _productService;

    public ProductsController(IProductService productService)
    {
        _productService = productService;
    }

    /// <summary>
    /// Lấy tất cả sản phẩm
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<ApiResponse<List<ProductDto>>>> GetAllProducts()
    {
        var result = await _productService.GetAllProductsAsync();
        return Ok(result);
    }

    /// <summary>
    /// Lấy sản phẩm theo danh mục
    /// </summary>
    [HttpGet("category/{categoryId}")]
    public async Task<ActionResult<ApiResponse<List<ProductDto>>>> GetProductsByCategory(int categoryId)
    {
        var result = await _productService.GetProductsByCategoryAsync(categoryId);
        return Ok(result);
    }

    /// <summary>
    /// Lấy chi tiết sản phẩm
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<ProductDto>>> GetProductById(int id)
    {
        var result = await _productService.GetProductByIdAsync(id);
        
        if (!result.Success)
        {
            return NotFound(result);
        }

        return Ok(result);
    }

    /// <summary>
    /// Lấy tất cả danh mục
    /// </summary>
    [HttpGet("categories")]
    public async Task<ActionResult<ApiResponse<List<CategoryDto>>>> GetAllCategories()
    {
        var result = await _productService.GetAllCategoriesAsync();
        return Ok(result);
    }

    /// <summary>
    /// Lấy tất cả sizes
    /// </summary>
    [HttpGet("sizes")]
    public async Task<ActionResult<ApiResponse<List<SizeDto>>>> GetAllSizes()
    {
        var result = await _productService.GetAllSizesAsync();
        return Ok(result);
    }

    /// <summary>
    /// Lấy tất cả toppings
    /// </summary>
    [HttpGet("toppings")]
    public async Task<ActionResult<ApiResponse<List<ToppingDto>>>> GetAllToppings()
    {
        var result = await _productService.GetAllToppingsAsync();
        return Ok(result);
    }
}
