using System.Collections.Generic;
using WebBanNuoc.Models.DTOs;
using WebBanNuoc.Models.Entities;

namespace WebBanNuoc.Services
{
    /// <summary>
    /// Service Pattern - Interface cho Product Service
    /// Chứa business logic liên quan đến Product
    /// </summary>
    public interface IProductService
    {
        // Product Operations
        ProductDTO GetProductById(int id);
        IEnumerable<ProductDTO> GetAllProducts();
        IEnumerable<ProductDTO> GetProductsByCategory(string categoryId);
        IEnumerable<ProductDTO> GetAvailableProducts();
        IEnumerable<ProductDTO> SearchProducts(string keyword);
        
        // Featured Products
        IEnumerable<ProductDTO> GetTopSellingProducts(int count);
        IEnumerable<ProductDTO> GetNewArrivals(int count);
        
        // Price Calculation with Strategy Pattern
        decimal CalculateProductPrice(int productId, string size, List<int> toppingIds);
        
        // Product Management
        void CreateProduct(Product product);
        void UpdateProduct(Product product);
        void DeleteProduct(int id);
        bool IsProductAvailable(int id);
    }
}
