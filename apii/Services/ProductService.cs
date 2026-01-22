using apii.Models.DTOs;
using apii.Models.Entities;
using apii.Repositories;
using Microsoft.EntityFrameworkCore;

namespace apii.Services;

/// <summary>
/// Interface cho Product Service
/// </summary>
public interface IProductService
{
    Task<ApiResponse<List<ProductDto>>> GetAllProductsAsync();
    Task<ApiResponse<List<ProductDto>>> GetProductsByCategoryAsync(int categoryId);
    Task<ApiResponse<ProductDto>> GetProductByIdAsync(int id);
    Task<ApiResponse<List<CategoryDto>>> GetAllCategoriesAsync();
    Task<ApiResponse<List<SizeDto>>> GetAllSizesAsync();
    Task<ApiResponse<List<ToppingDto>>> GetAllToppingsAsync();
}

/// <summary>
/// Product Service
/// </summary>
public class ProductService : IProductService
{
    private readonly IUnitOfWork _unitOfWork;

    public ProductService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ApiResponse<List<ProductDto>>> GetAllProductsAsync()
    {
        try
        {
            var products = await _unitOfWork.Repository<Product>()
                .FindAsync(p => p.IsAvailable);

            var productDtos = new List<ProductDto>();
            foreach (var product in products)
            {
                var category = await _unitOfWork.Repository<Category>().GetByIdAsync(product.CategoryId);
                productDtos.Add(new ProductDto
                {
                    Id = product.Id,
                    Name = product.Name,
                    Description = product.Description,
                    BasePrice = product.BasePrice,
                    CategoryId = product.CategoryId,
                    CategoryName = category?.Name,
                    ImageUrl = product.ImageUrl,
                    IsAvailable = product.IsAvailable
                });
            }

            return ApiResponse<List<ProductDto>>.SuccessResponse(productDtos, "Lấy danh sách sản phẩm thành công");
        }
        catch (Exception ex)
        {
            return ApiResponse<List<ProductDto>>.ErrorResponse($"Lỗi: {ex.Message}");
        }
    }

    public async Task<ApiResponse<List<ProductDto>>> GetProductsByCategoryAsync(int categoryId)
    {
        try
        {
            var products = await _unitOfWork.Repository<Product>()
                .FindAsync(p => p.CategoryId == categoryId && p.IsAvailable);

            var category = await _unitOfWork.Repository<Category>().GetByIdAsync(categoryId);

            var productDtos = products.Select(p => new ProductDto
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                BasePrice = p.BasePrice,
                CategoryId = p.CategoryId,
                CategoryName = category?.Name,
                ImageUrl = p.ImageUrl,
                IsAvailable = p.IsAvailable
            }).ToList();

            return ApiResponse<List<ProductDto>>.SuccessResponse(productDtos, "Lấy sản phẩm theo danh mục thành công");
        }
        catch (Exception ex)
        {
            return ApiResponse<List<ProductDto>>.ErrorResponse($"Lỗi: {ex.Message}");
        }
    }

    public async Task<ApiResponse<ProductDto>> GetProductByIdAsync(int id)
    {
        try
        {
            var product = await _unitOfWork.Repository<Product>().GetByIdAsync(id);
            if (product == null)
            {
                return ApiResponse<ProductDto>.ErrorResponse("Không tìm thấy sản phẩm");
            }

            var category = await _unitOfWork.Repository<Category>().GetByIdAsync(product.CategoryId);

            var productDto = new ProductDto
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                BasePrice = product.BasePrice,
                CategoryId = product.CategoryId,
                CategoryName = category?.Name,
                ImageUrl = product.ImageUrl,
                IsAvailable = product.IsAvailable
            };

            return ApiResponse<ProductDto>.SuccessResponse(productDto, "Lấy thông tin sản phẩm thành công");
        }
        catch (Exception ex)
        {
            return ApiResponse<ProductDto>.ErrorResponse($"Lỗi: {ex.Message}");
        }
    }

    public async Task<ApiResponse<List<CategoryDto>>> GetAllCategoriesAsync()
    {
        try
        {
            var categories = await _unitOfWork.Repository<Category>()
                .FindAsync(c => c.IsActive);

            var categoryDtos = categories
                .OrderBy(c => c.DisplayOrder)
                .Select(c => new CategoryDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    Slug = c.Slug,
                    DisplayOrder = c.DisplayOrder
                }).ToList();

            return ApiResponse<List<CategoryDto>>.SuccessResponse(categoryDtos, "Lấy danh mục thành công");
        }
        catch (Exception ex)
        {
            return ApiResponse<List<CategoryDto>>.ErrorResponse($"Lỗi: {ex.Message}");
        }
    }

    public async Task<ApiResponse<List<SizeDto>>> GetAllSizesAsync()
    {
        try
        {
            var sizes = await _unitOfWork.Repository<Size>().GetAllAsync();

            var sizeDtos = sizes.Select(s => new SizeDto
            {
                Id = s.Id,
                Code = s.Code,
                Name = s.Name,
                AdditionalPrice = s.AdditionalPrice
            }).ToList();

            return ApiResponse<List<SizeDto>>.SuccessResponse(sizeDtos, "Lấy danh sách size thành công");
        }
        catch (Exception ex)
        {
            return ApiResponse<List<SizeDto>>.ErrorResponse($"Lỗi: {ex.Message}");
        }
    }

    public async Task<ApiResponse<List<ToppingDto>>> GetAllToppingsAsync()
    {
        try
        {
            var toppings = await _unitOfWork.Repository<Topping>()
                .FindAsync(t => t.IsAvailable);

            var toppingDtos = toppings.Select(t => new ToppingDto
            {
                Id = t.Id,
                Name = t.Name,
                Price = t.Price,
                IsAvailable = t.IsAvailable
            }).ToList();

            return ApiResponse<List<ToppingDto>>.SuccessResponse(toppingDtos, "Lấy danh sách topping thành công");
        }
        catch (Exception ex)
        {
            return ApiResponse<List<ToppingDto>>.ErrorResponse($"Lỗi: {ex.Message}");
        }
    }
}
