using System;
using System.Collections.Generic;
using System.Linq;
using WebBanNuoc.Models.DTOs;
using WebBanNuoc.Models.Entities;
using WebBanNuoc.Models.Strategies;

namespace WebBanNuoc.Services.Implementations
{
    /// <summary>
    /// Local Product Service với dữ liệu tĩnh
    /// Thay thế API calls bằng dữ liệu mẫu
    /// </summary>
    public class LocalProductService : IProductService
    {
        private readonly IPricingStrategy _pricingStrategy;
        private static List<ProductDTO> _products;

        public LocalProductService(IPricingStrategy pricingStrategy)
        {
            _pricingStrategy = pricingStrategy;
            InitializeProducts();
        }

        private void InitializeProducts()
        {
            if (_products == null)
            {
                _products = new List<ProductDTO>
                {
                    new ProductDTO
                    {
                        Id = 1,
                        Name = "Nước khoáng Lavie 500ml",
                        Description = "Nước khoáng thiên nhiên Lavie, chai 500ml tiện lợi",
                        BasePrice = 5000,
                        CategoryName = "Nước khoáng",
                        ImageUrl = "/Content/images/lavie-500ml.jpg",
                        ReviewCount = 120,
                        AverageRating = 4.5m
                    },
                    new ProductDTO
                    {
                        Id = 2,
                        Name = "Nước khoáng Aquafina 500ml",
                        Description = "Nước tinh khiết Aquafina, sạch và an toàn",
                        BasePrice = 5000,
                        CategoryName = "Nước khoáng",
                        ImageUrl = "/Content/images/aquafina-500ml.jpg",
                        ReviewCount = 95,
                        AverageRating = 4.3m
                    },
                    new ProductDTO
                    {
                        Id = 3,
                        Name = "Nước khoáng Dasani 500ml",
                        Description = "Nước tinh khiết Dasani với hương vị tươi mát",
                        BasePrice = 6000,
                        CategoryName = "Nước khoáng",
                        ImageUrl = "/Content/images/dasani-500ml.jpg",
                        ReviewCount = 80,
                        AverageRating = 4.2m
                    },
                    new ProductDTO
                    {
                        Id = 4,
                        Name = "Nước suối Vĩnh Hảo 500ml",
                        Description = "Nước suối thiên nhiên từ nguồn Vĩnh Hảo",
                        BasePrice = 4500,
                        CategoryName = "Nước khoáng",
                        ImageUrl = "/Content/images/vinhhao-500ml.jpg",
                        ReviewCount = 150,
                        AverageRating = 4.6m
                    },
                    new ProductDTO
                    {
                        Id = 5,
                        Name = "Nước tinh khiết Miru 500ml",
                        Description = "Nước tinh khiết Miru, sạch và trong lành",
                        BasePrice = 4000,
                        CategoryName = "Nước khoáng",
                        ImageUrl = "/Content/images/miru-500ml.jpg",
                        ReviewCount = 70,
                        AverageRating = 4.1m
                    },
                    new ProductDTO
                    {
                        Id = 6,
                        Name = "Nước khoáng Evian 500ml",
                        Description = "Nước khoáng cao cấp từ Pháp",
                        BasePrice = 35000,
                        CategoryName = "Nước khoáng cao cấp",
                        ImageUrl = "/Content/images/evian-500ml.jpg",
                        ReviewCount = 45,
                        AverageRating = 4.8m
                    },
                    new ProductDTO
                    {
                        Id = 7,
                        Name = "Nước suối Lavie 1.5L",
                        Description = "Nước khoáng thiên nhiên Lavie, chai 1.5L gia đình",
                        BasePrice = 10000,
                        CategoryName = "Nước khoáng",
                        ImageUrl = "/Content/images/lavie-1500ml.jpg",
                        ReviewCount = 200,
                        AverageRating = 4.7m
                    },
                    new ProductDTO
                    {
                        Id = 8,
                        Name = "Nước khoáng Aquafina 1.5L",
                        Description = "Nước tinh khiết Aquafina, chai lớn 1.5L",
                        BasePrice = 10000,
                        CategoryName = "Nước khoáng",
                        ImageUrl = "/Content/images/aquafina-1500ml.jpg",
                        ReviewCount = 180,
                        AverageRating = 4.6m
                    },
                    new ProductDTO
                    {
                        Id = 9,
                        Name = "Nước có ga Sprite 330ml",
                        Description = "Nước ngọt có ga Sprite, vị chanh tươi mát",
                        BasePrice = 8000,
                        CategoryName = "Nước có ga",
                        ImageUrl = "/Content/images/sprite-330ml.jpg",
                        ReviewCount = 250,
                        AverageRating = 4.4m
                    },
                    new ProductDTO
                    {
                        Id = 10,
                        Name = "Nước có ga Coca-Cola 330ml",
                        Description = "Nước ngọt có ga Coca-Cola, hương vị đặc trưng",
                        BasePrice = 8000,
                        CategoryName = "Nước có ga",
                        ImageUrl = "/Content/images/coca-330ml.jpg",
                        ReviewCount = 300,
                        AverageRating = 4.5m
                    },
                    new ProductDTO
                    {
                        Id = 11,
                        Name = "Nước có ga Pepsi 330ml",
                        Description = "Nước ngọt có ga Pepsi, sảng khoái",
                        BasePrice = 8000,
                        CategoryName = "Nước có ga",
                        ImageUrl = "/Content/images/pepsi-330ml.jpg",
                        ReviewCount = 220,
                        AverageRating = 4.3m
                    },
                    new ProductDTO
                    {
                        Id = 12,
                        Name = "Nước ép cam Minute Maid 1L",
                        Description = "Nước ép cam tươi Minute Maid, giàu vitamin C",
                        BasePrice = 25000,
                        CategoryName = "Nước ép trái cây",
                        ImageUrl = "/Content/images/minutemaid-1000ml.jpg",
                        ReviewCount = 130,
                        AverageRating = 4.6m
                    }
                };
            }
        }

        public ProductDTO GetProductById(int id)
        {
            return _products.FirstOrDefault(p => p.Id == id);
        }

        public IEnumerable<ProductDTO> GetAllProducts()
        {
            return _products.ToList();
        }

        public IEnumerable<ProductDTO> GetProductsByCategory(string categoryId)
        {
            if (string.IsNullOrEmpty(categoryId))
            {
                return _products.ToList();
            }
            return _products.Where(p => p.CategoryName.Contains(categoryId)).ToList();
        }

        public IEnumerable<ProductDTO> GetAvailableProducts()
        {
            return _products.ToList();
        }

        public IEnumerable<ProductDTO> SearchProducts(string keyword)
        {
            if (string.IsNullOrEmpty(keyword))
            {
                return _products.ToList();
            }
            
            keyword = keyword.ToLower();
            return _products.Where(p => 
                p.Name.ToLower().Contains(keyword) || 
                p.Description.ToLower().Contains(keyword) ||
                p.CategoryName.ToLower().Contains(keyword)
            ).ToList();
        }

        public IEnumerable<ProductDTO> GetTopSellingProducts(int count)
        {
            // Sắp xếp theo số lượng review (giả định là bán chạy)
            return _products.OrderByDescending(p => p.ReviewCount).Take(count).ToList();
        }

        public IEnumerable<ProductDTO> GetNewArrivals(int count)
        {
            // Lấy sản phẩm mới nhất (giả định ID càng cao càng mới)
            return _products.OrderByDescending(p => p.Id).Take(count).ToList();
        }

        public decimal CalculateProductPrice(int productId, string size, List<int> toppingIds)
        {
            var product = GetProductById(productId);
            if (product == null)
                throw new Exception("Product not found");

            // Tạo Product entity tạm thời để tính giá
            var productEntity = new Product
            {
                Id = product.Id,
                Name = product.Name,
                BasePrice = product.BasePrice
            };

            var options = new CustomizationOptions
            {
                Size = GetSize(size),
                Toppings = GetToppings(toppingIds),
                Quantity = 1
            };

            return _pricingStrategy.CalculatePrice(productEntity, options);
        }

        public void CreateProduct(Product product)
        {
            var newProduct = new ProductDTO
            {
                Id = _products.Max(p => p.Id) + 1,
                Name = product.Name,
                Description = product.Description,
                BasePrice = product.BasePrice,
                CategoryName = product.CategoryId,
                ImageUrl = product.ImageUrl,
                ReviewCount = 0,
                AverageRating = 0
            };
            _products.Add(newProduct);
        }

        public void UpdateProduct(Product product)
        {
            var existingProduct = _products.FirstOrDefault(p => p.Id == product.Id);
            if (existingProduct != null)
            {
                existingProduct.Name = product.Name;
                existingProduct.Description = product.Description;
                existingProduct.BasePrice = product.BasePrice;
                existingProduct.CategoryName = product.CategoryId;
                existingProduct.ImageUrl = product.ImageUrl;
            }
        }

        public void DeleteProduct(int id)
        {
            var product = _products.FirstOrDefault(p => p.Id == id);
            if (product != null)
            {
                _products.Remove(product);
            }
        }

        public bool IsProductAvailable(int id)
        {
            return _products.Any(p => p.Id == id);
        }

        private Size GetSize(string sizeCode)
        {
            var sizes = new Dictionary<string, Size>
            {
                { "S", new Size { Code = "S", Name = "Small", AdditionalPrice = 0 } },
                { "M", new Size { Code = "M", Name = "Medium", AdditionalPrice = 5000 } },
                { "L", new Size { Code = "L", Name = "Large", AdditionalPrice = 10000 } }
            };

            return sizes.ContainsKey(sizeCode) ? sizes[sizeCode] : sizes["M"];
        }

        private List<Topping> GetToppings(List<int> toppingIds)
        {
            var allToppings = new List<Topping>
            {
                new Topping { Id = 1, Name = "Trân châu", Price = 5000 },
                new Topping { Id = 2, Name = "Thạch", Price = 5000 },
                new Topping { Id = 3, Name = "Pudding", Price = 7000 },
                new Topping { Id = 4, Name = "Trân châu hoàng kim", Price = 8000 }
            };

            return allToppings.Where(t => toppingIds.Contains(t.Id)).ToList();
        }
    }
}
