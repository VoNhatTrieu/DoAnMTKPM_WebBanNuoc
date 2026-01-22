using apii.Models.DTOs;
using apii.Models.Entities;
using apii.Repositories;

namespace apii.Services;

/// <summary>
/// Interface cho Cart Service
/// </summary>
public interface ICartService
{
    Task<ApiResponse<CartSummaryDto>> GetCartAsync(int? userId, string? sessionId);
    Task<ApiResponse<string>> AddToCartAsync(AddToCartDto dto, int? userId, string? sessionId);
    Task<ApiResponse<string>> UpdateCartItemAsync(int cartItemId, int quantity);
    Task<ApiResponse<string>> RemoveCartItemAsync(int cartItemId);
    Task<ApiResponse<string>> ClearCartAsync(int? userId, string? sessionId);
}

/// <summary>
/// Cart Service với Strategy Pattern cho tính phí ship
/// </summary>
public class CartService : ICartService
{
    private readonly IUnitOfWork _unitOfWork;

    public CartService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ApiResponse<CartSummaryDto>> GetCartAsync(int? userId, string? sessionId)
    {
        try
        {
            var cartItems = await GetCartItemsFromDb(userId, sessionId);

            var cartItemDtos = cartItems.Select(c => new CartItemDto
            {
                Id = c.Id,
                ProductId = c.ProductId,
                ProductName = c.ProductName,
                ImageUrl = c.ImageUrl,
                UnitPrice = c.UnitPrice,
                Quantity = c.Quantity,
                Size = c.Size,
                SugarLevel = c.SugarLevel,
                IceLevel = c.IceLevel,
                Toppings = c.Toppings
            }).ToList();

            var summary = new CartSummaryDto
            {
                Items = cartItemDtos,
                Subtotal = cartItemDtos.Sum(i => i.TotalPrice),
            };

            summary.ShippingFee = CalculateShippingFee(summary.Subtotal);
            summary.Discount = 0; // TODO: Apply voucher logic
            summary.Total = summary.Subtotal + summary.ShippingFee - summary.Discount;

            return ApiResponse<CartSummaryDto>.SuccessResponse(summary, "Lấy giỏ hàng thành công");
        }
        catch (Exception ex)
        {
            return ApiResponse<CartSummaryDto>.ErrorResponse($"Lỗi: {ex.Message}");
        }
    }

    public async Task<ApiResponse<string>> AddToCartAsync(AddToCartDto dto, int? userId, string? sessionId)
    {
        try
        {
            var product = await _unitOfWork.Repository<Product>().GetByIdAsync(dto.ProductId);
            if (product == null)
            {
                return ApiResponse<string>.ErrorResponse("Không tìm thấy sản phẩm");
            }

            var cartItem = new Cart
            {
                UserId = userId,
                SessionId = sessionId,
                ProductId = product.Id,
                ProductName = product.Name,
                ImageUrl = product.ImageUrl,
                UnitPrice = product.BasePrice,
                Quantity = dto.Quantity,
                Size = dto.Size,
                SugarLevel = dto.SugarLevel,
                IceLevel = dto.IceLevel,
                Toppings = dto.Toppings != null ? string.Join(",", dto.Toppings) : null,
                CreatedDate = DateTime.Now
            };

            await _unitOfWork.Repository<Cart>().AddAsync(cartItem);
            await _unitOfWork.SaveChangesAsync();

            return ApiResponse<string>.SuccessResponse("OK", "Thêm vào giỏ hàng thành công");
        }
        catch (Exception ex)
        {
            return ApiResponse<string>.ErrorResponse($"Lỗi: {ex.Message}");
        }
    }

    public async Task<ApiResponse<string>> UpdateCartItemAsync(int cartItemId, int quantity)
    {
        try
        {
            var cartItem = await _unitOfWork.Repository<Cart>().GetByIdAsync(cartItemId);
            if (cartItem == null)
            {
                return ApiResponse<string>.ErrorResponse("Không tìm thấy sản phẩm trong giỏ hàng");
            }

            if (quantity <= 0)
            {
                return ApiResponse<string>.ErrorResponse("Số lượng phải lớn hơn 0");
            }

            cartItem.Quantity = quantity;
            _unitOfWork.Repository<Cart>().Update(cartItem);
            await _unitOfWork.SaveChangesAsync();

            return ApiResponse<string>.SuccessResponse("OK", "Cập nhật giỏ hàng thành công");
        }
        catch (Exception ex)
        {
            return ApiResponse<string>.ErrorResponse($"Lỗi: {ex.Message}");
        }
    }

    public async Task<ApiResponse<string>> RemoveCartItemAsync(int cartItemId)
    {
        try
        {
            var cartItem = await _unitOfWork.Repository<Cart>().GetByIdAsync(cartItemId);
            if (cartItem == null)
            {
                return ApiResponse<string>.ErrorResponse("Không tìm thấy sản phẩm trong giỏ hàng");
            }

            _unitOfWork.Repository<Cart>().Remove(cartItem);
            await _unitOfWork.SaveChangesAsync();

            return ApiResponse<string>.SuccessResponse("OK", "Xóa sản phẩm khỏi giỏ hàng thành công");
        }
        catch (Exception ex)
        {
            return ApiResponse<string>.ErrorResponse($"Lỗi: {ex.Message}");
        }
    }

    public async Task<ApiResponse<string>> ClearCartAsync(int? userId, string? sessionId)
    {
        try
        {
            var cartItems = await GetCartItemsFromDb(userId, sessionId);
            _unitOfWork.Repository<Cart>().RemoveRange(cartItems);
            await _unitOfWork.SaveChangesAsync();

            return ApiResponse<string>.SuccessResponse("OK", "Xóa giỏ hàng thành công");
        }
        catch (Exception ex)
        {
            return ApiResponse<string>.ErrorResponse($"Lỗi: {ex.Message}");
        }
    }

    private async Task<IEnumerable<Cart>> GetCartItemsFromDb(int? userId, string? sessionId)
    {
        if (userId.HasValue)
        {
            return await _unitOfWork.Repository<Cart>().FindAsync(c => c.UserId == userId);
        }
        else if (!string.IsNullOrEmpty(sessionId))
        {
            return await _unitOfWork.Repository<Cart>().FindAsync(c => c.SessionId == sessionId);
        }
        return new List<Cart>();
    }

    /// <summary>
    /// Strategy Pattern - Tính phí ship
    /// </summary>
    private decimal CalculateShippingFee(decimal subtotal)
    {
        // Miễn phí ship cho đơn >= 100,000đ
        if (subtotal >= 100000)
            return 0;

        // Phí ship cố định 20,000đ
        return 20000;
    }
}
