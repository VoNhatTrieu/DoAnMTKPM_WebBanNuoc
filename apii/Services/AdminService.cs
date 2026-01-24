using apii.Data;
using apii.Models.DTOs;
using apii.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace apii.Services;

/// <summary>
/// Interface for Admin Service
/// </summary>
public interface IAdminService
{
    Task<DashboardStatsDto> GetDashboardStatsAsync(string period);
    Task<List<RevenueChartDto>> GetRevenueChartAsync();
    Task<List<TopProductDto>> GetTopProductsAsync(int limit);
    Task<List<AdminOrderDto>> GetRecentOrdersAsync(int limit);
    Task<PagedResult<AdminOrderDto>> GetAllOrdersAsync(int page, int pageSize, string? status);
    Task<AdminOrderDetailDto?> GetOrderDetailAsync(int orderId);
    Task<bool> UpdateOrderStatusAsync(int orderId, string status);
    Task<List<CustomerDto>> GetAllCustomersAsync();
    Task<List<AdminProductDto>> GetAllProductsAsync();
    Task<AdminProductDto?> GetProductByIdAsync(int productId);
    Task<Product> CreateProductAsync(CreateProductDto dto);
    Task<Product?> UpdateProductAsync(int productId, UpdateProductDto dto);
    Task<bool> DeleteProductAsync(int productId);
}

/// <summary>
/// Admin Service - Business logic for admin operations
/// </summary>
public class AdminService : IAdminService
{
    private readonly AppDbContext _context;

    public AdminService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<DashboardStatsDto> GetDashboardStatsAsync(string period)
    {
        var now = DateTime.Now;
        DateTime startDate = period switch
        {
            "today" => now.Date,
            "week" => now.AddDays(-7),
            "month" => now.AddMonths(-1),
            _ => now.Date
        };

        var orders = await _context.Orders
            .Where(o => o.OrderDate >= startDate)
            .ToListAsync();

        var stats = new DashboardStatsDto
        {
            TotalOrders = orders.Count,
            CompletedOrders = orders.Count(o => o.Status == "Delivered"),
            PendingOrders = orders.Count(o => o.Status == "Pending"),
            Revenue = orders.Where(o => o.Status == "Delivered").Sum(o => o.Total),
            TotalCustomers = await _context.Users.CountAsync(u => u.Role == "Customer"),
            NewCustomers = await _context.Users.CountAsync(u => u.Role == "Customer" && u.CreatedDate >= startDate),
            TotalProducts = await _context.Products.CountAsync()
        };

        return stats;
    }

    public async Task<List<RevenueChartDto>> GetRevenueChartAsync()
    {
        var startDate = DateTime.Now.AddDays(-7).Date;
        var orders = await _context.Orders
            .Where(o => o.OrderDate >= startDate && o.Status == "Delivered")
            .ToListAsync();

        var revenueData = orders
            .GroupBy(o => o.OrderDate.Date)
            .Select(g => new RevenueChartDto
            {
                Date = g.Key.ToString("dd/MM"),
                Revenue = g.Sum(o => o.Total)
            })
            .OrderBy(r => r.Date)
            .ToList();

        return revenueData;
    }

    public async Task<List<TopProductDto>> GetTopProductsAsync(int limit)
    {
        var topProducts = await _context.OrderDetails
            .Include(od => od.Product)
            .GroupBy(od => new { od.ProductId, od.Product.Name })
            .Select(g => new TopProductDto
            {
                ProductName = g.Key.Name,
                TotalQuantity = g.Sum(od => od.Quantity),
                TotalRevenue = g.Sum(od => od.Quantity * od.UnitPrice)
            })
            .OrderByDescending(p => p.TotalQuantity)
            .Take(limit)
            .ToListAsync();

        return topProducts;
    }

    public async Task<List<AdminOrderDto>> GetRecentOrdersAsync(int limit)
    {
        var orders = await _context.Orders
            .Include(o => o.User)
            .OrderByDescending(o => o.OrderDate)
            .Take(limit)
            .Select(o => new AdminOrderDto
            {
                OrderId = o.Id,
                UserId = o.UserId,
                CustomerName = o.User != null ? o.User.FullName : o.CustomerName,
                TotalAmount = o.Total,
                Status = o.Status,
                CreatedAt = o.OrderDate
            })
            .ToListAsync();

        return orders;
    }

    public async Task<PagedResult<AdminOrderDto>> GetAllOrdersAsync(int page, int pageSize, string? status)
    {
        var query = _context.Orders
            .Include(o => o.User)
            .AsQueryable();

        if (!string.IsNullOrEmpty(status))
        {
            query = query.Where(o => o.Status == status);
        }

        var totalItems = await query.CountAsync();
        var orders = await query
            .OrderByDescending(o => o.OrderDate)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(o => new AdminOrderDto
            {
                OrderId = o.Id,
                UserId = o.UserId,
                CustomerName = o.User != null ? o.User.FullName : o.CustomerName,
                CustomerPhone = o.CustomerPhone,
                CustomerAddress = o.ShippingAddress,
                TotalAmount = o.Total,
                Status = o.Status,
                CreatedAt = o.OrderDate
            })
            .ToListAsync();

        return new PagedResult<AdminOrderDto>
        {
            Items = orders,
            TotalItems = totalItems,
            Page = page,
            PageSize = pageSize
        };
    }

    public async Task<AdminOrderDetailDto?> GetOrderDetailAsync(int orderId)
    {
        var order = await _context.Orders
            .Include(o => o.User)
            .Include(o => o.OrderDetails)
                .ThenInclude(od => od.Product)
            .FirstOrDefaultAsync(o => o.Id == orderId);

        if (order == null)
            return null;

        return new AdminOrderDetailDto
        {
            OrderId = order.Id,
            CustomerName = order.User != null ? order.User.FullName : order.CustomerName,
            CustomerEmail = order.User != null ? order.User.Email : order.CustomerEmail ?? "",
            CustomerPhone = order.CustomerPhone,
            DeliveryAddress = order.ShippingAddress,
            TotalAmount = order.Total,
            Status = order.Status,
            CreatedAt = order.OrderDate,
            Items = order.OrderDetails.Select(od => new AdminOrderItemDto
            {
                ProductName = od.Product.Name,
                Quantity = od.Quantity,
                UnitPrice = od.UnitPrice,
                Size = od.Size,
                Toppings = od.Toppings
            }).ToList()
        };
    }

    public async Task<bool> UpdateOrderStatusAsync(int orderId, string status)
    {
        var order = await _context.Orders.FindAsync(orderId);
        if (order == null)
            return false;

        order.Status = status;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<List<CustomerDto>> GetAllCustomersAsync()
    {
        var customers = await _context.Users
            .Where(u => u.Role == "Customer")
            .Select(u => new CustomerDto
            {
                UserId = u.Id,
                FullName = u.FullName,
                Email = u.Email,
                CreatedAt = u.CreatedDate,
                TotalOrders = _context.Orders.Count(o => o.UserId == u.Id),
                TotalSpent = _context.Orders
                    .Where(o => o.UserId == u.Id && o.Status == "Delivered")
                    .Sum(o => (decimal?)o.Total) ?? 0
            })
            .OrderByDescending(c => c.TotalSpent)
            .ToListAsync();

        return customers;
    }

    public async Task<List<AdminProductDto>> GetAllProductsAsync()
    {
        var products = await _context.Products
            .Include(p => p.Category)
            .Select(p => new AdminProductDto
            {
                ProductId = p.Id,
                Name = p.Name,
                Description = p.Description ?? "",
                BasePrice = p.BasePrice,
                ImageUrl = p.ImageUrl ?? "",
                CategoryId = p.CategoryId,
                CategoryName = p.Category.Name,
                IsActive = p.IsAvailable,
                CreatedAt = p.CreatedDate
            })
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync();

        return products;
    }

    public async Task<AdminProductDto?> GetProductByIdAsync(int productId)
    {
        var product = await _context.Products
            .Include(p => p.Category)
            .Where(p => p.Id == productId)
            .Select(p => new AdminProductDto
            {
                ProductId = p.Id,
                Name = p.Name,
                Description = p.Description ?? "",
                BasePrice = p.BasePrice,
                ImageUrl = p.ImageUrl ?? "",
                CategoryId = p.CategoryId,
                CategoryName = p.Category.Name,
                IsActive = p.IsAvailable,
                CreatedAt = p.CreatedDate
            })
            .FirstOrDefaultAsync();

        return product;
    }

    public async Task<Product> CreateProductAsync(CreateProductDto dto)
    {
        var product = new Product
        {
            Name = dto.Name,
            Description = dto.Description,
            BasePrice = dto.BasePrice,
            ImageUrl = dto.ImageUrl,
            CategoryId = dto.CategoryId,
            IsAvailable = dto.IsAvailable,
            CreatedDate = DateTime.Now
        };

        _context.Products.Add(product);
        await _context.SaveChangesAsync();
        return product;
    }

    public async Task<Product?> UpdateProductAsync(int productId, UpdateProductDto dto)
    {
        var product = await _context.Products.FindAsync(productId);
        if (product == null)
            return null;

        product.Name = dto.Name;
        product.Description = dto.Description;
        product.BasePrice = dto.BasePrice;
        product.ImageUrl = dto.ImageUrl;
        product.CategoryId = dto.CategoryId;
        product.IsAvailable = dto.IsAvailable;

        await _context.SaveChangesAsync();
        return product;
    }

    public async Task<bool> DeleteProductAsync(int productId)
    {
        var product = await _context.Products.FindAsync(productId);
        if (product == null)
            return false;

        _context.Products.Remove(product);
        await _context.SaveChangesAsync();
        return true;
    }
}
