using apii.Data;
using apii.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace apii.Services
{
    public class SeedDataService
    {
        private readonly AppDbContext _context;

        public SeedDataService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<int> SeedProductsAsync()
        {
            // Kiểm tra nếu đã có sản phẩm thì không seed nữa
            if (await _context.Products.AnyAsync())
            {
                return 0;
            }

            var products = new List<Product>
            {
                // TRÀ SỮA (CategoryId = 1)
                new Product { Name = "Trà Sữa Truyền Thống", Description = "Trà sữa đậm đà, thơm ngon với trân châu đen", BasePrice = 35000, CategoryId = 1, ImageUrl = "/images/products/trasua-truyenthong.jpg", IsAvailable = true },
                new Product { Name = "Trà Sữa Matcha", Description = "Trà sữa matcha Nhật Bản, vị đắng nhẹ hòa quyện", BasePrice = 40000, CategoryId = 1, ImageUrl = "/images/products/trasua-matcha.jpg", IsAvailable = true },
                new Product { Name = "Trà Sữa Socola", Description = "Trà sữa socola ngọt ngào, béo ngậy", BasePrice = 38000, CategoryId = 1, ImageUrl = "/images/products/trasua-socola.jpg", IsAvailable = true },
                new Product { Name = "Trà Sữa Oolong", Description = "Trà sữa oolong thơm dịu, thanh mát", BasePrice = 42000, CategoryId = 1, ImageUrl = "/images/products/trasua-oolong.jpg", IsAvailable = true },

                // TRÀ (CategoryId = 2)
                new Product { Name = "Trà Đào Cam Sả", Description = "Trà đào cam sả sảng khoái, giải nhiệt", BasePrice = 38000, CategoryId = 2, ImageUrl = "/images/products/tra-dao-cam-sa.jpg", IsAvailable = true },
                new Product { Name = "Trà Ổi Hồng", Description = "Trà ổi hồng thanh mát, giàu vitamin C", BasePrice = 35000, CategoryId = 2, ImageUrl = "/images/products/tra-oi-hong.jpg", IsAvailable = true },
                new Product { Name = "Trà Chanh Dây", Description = "Trà chanh dây chua ngọt, sảng khoái", BasePrice = 32000, CategoryId = 2, ImageUrl = "/images/products/tra-chanh-day.jpg", IsAvailable = true },
                new Product { Name = "Trà Vải", Description = "Trà vải thơm ngon, ngọt mát", BasePrice = 36000, CategoryId = 2, ImageUrl = "/images/products/tra-vai.jpg", IsAvailable = true },

                // TRÁI CÂY (CategoryId = 3)
                new Product { Name = "Nước Dừa Tươi", Description = "Nước dừa tươi nguyên chất, mát lạnh", BasePrice = 25000, CategoryId = 3, ImageUrl = "/images/products/nuoc-dua.jpg", IsAvailable = true },
                new Product { Name = "Cam Vắt", Description = "Nước cam vắt tươi ngon, giàu vitamin", BasePrice = 30000, CategoryId = 3, ImageUrl = "/images/products/cam-vat.jpg", IsAvailable = true },
                new Product { Name = "Chanh Muối", Description = "Chanh muối giải khát, thanh nhiệt", BasePrice = 28000, CategoryId = 3, ImageUrl = "/images/products/chanh-muoi.jpg", IsAvailable = true },

                // CÀ PHÊ (CategoryId = 4)
                new Product { Name = "Cà Phê Sữa Đá", Description = "Cà phê phin truyền thống Việt Nam", BasePrice = 30000, CategoryId = 4, ImageUrl = "/images/products/caphe-suada.jpg", IsAvailable = true },
                new Product { Name = "Bạc Xỉu", Description = "Cà phê sữa ngọt ngào, nhẹ nhàng", BasePrice = 32000, CategoryId = 4, ImageUrl = "/images/products/bacxiu.jpg", IsAvailable = true },
                new Product { Name = "Cà Phê Đen Đá", Description = "Cà phê đen đậm đà, mạnh mẽ", BasePrice = 28000, CategoryId = 4, ImageUrl = "/images/products/caphe-den.jpg", IsAvailable = true },
                new Product { Name = "Cappuccino", Description = "Cà phê Ý với lớp sữa bọt mịn màng", BasePrice = 45000, CategoryId = 4, ImageUrl = "/images/products/cappuccino.jpg", IsAvailable = true },

                // SINH TỐ (CategoryId = 5)
                new Product { Name = "Sinh Tố Bơ", Description = "Sinh tố bơ béo ngậy, bổ dưỡng", BasePrice = 38000, CategoryId = 5, ImageUrl = "/images/products/sinhto-bo.jpg", IsAvailable = true },
                new Product { Name = "Sinh Tố Xoài", Description = "Sinh tố xoài thơm ngon, mát lạnh", BasePrice = 35000, CategoryId = 5, ImageUrl = "/images/products/sinhto-xoai.jpg", IsAvailable = true },
                new Product { Name = "Sinh Tố Dâu", Description = "Sinh tố dâu tươi ngọt mát", BasePrice = 40000, CategoryId = 5, ImageUrl = "/images/products/sinhto-dau.jpg", IsAvailable = true },
                new Product { Name = "Sinh Tố Sapoche", Description = "Sinh tố sapoche béo ngậy, thơm ngon", BasePrice = 36000, CategoryId = 5, ImageUrl = "/images/products/sinhto-sapoche.jpg", IsAvailable = true },

                // NƯỚC ÉP (CategoryId = 6)
                new Product { Name = "Nước Ép Dưa Hấu", Description = "Nước ép dưa hấu tươi mát, giải nhiệt", BasePrice = 30000, CategoryId = 6, ImageUrl = "/images/products/ep-dua-hau.jpg", IsAvailable = true },
                new Product { Name = "Nước Ép Cà Rót", Description = "Nước ép cà rốt giàu vitamin A", BasePrice = 32000, CategoryId = 6, ImageUrl = "/images/products/ep-ca-rot.jpg", IsAvailable = true },
                new Product { Name = "Nước Ép Táo", Description = "Nước ép táo tươi ngon, bổ dưỡng", BasePrice = 35000, CategoryId = 6, ImageUrl = "/images/products/ep-tao.jpg", IsAvailable = true },

                // SODA (CategoryId = 7)
                new Product { Name = "Soda Chanh", Description = "Soda chanh tươi sảng khoái", BasePrice = 28000, CategoryId = 7, ImageUrl = "/images/products/soda-chanh.jpg", IsAvailable = true },
                new Product { Name = "Soda Blue Hawaii", Description = "Soda xanh đẹp mắt, ngọt mát", BasePrice = 32000, CategoryId = 7, ImageUrl = "/images/products/soda-blue.jpg", IsAvailable = true },
                new Product { Name = "Soda Dâu", Description = "Soda dâu tươi ngọt ngào", BasePrice = 30000, CategoryId = 7, ImageUrl = "/images/products/soda-dau.jpg", IsAvailable = true },

                // BÁNH NGỌT (CategoryId = 8)
                new Product { Name = "Bánh Flan", Description = "Bánh flan mềm mịn, ngọt dịu", BasePrice = 25000, CategoryId = 8, ImageUrl = "/images/products/banh-flan.jpg", IsAvailable = true },
                new Product { Name = "Tiramisu", Description = "Tiramisu Ý nguyên bản, thơm cà phê", BasePrice = 45000, CategoryId = 8, ImageUrl = "/images/products/tiramisu.jpg", IsAvailable = true },
                new Product { Name = "Mousse Chocolate", Description = "Mousse chocolate mềm mịn, đắng nhẹ", BasePrice = 42000, CategoryId = 8, ImageUrl = "/images/products/mousse-choco.jpg", IsAvailable = true },
                new Product { Name = "Cheesecake", Description = "Cheesecake béo ngậy, ngọt ngào", BasePrice = 48000, CategoryId = 8, ImageUrl = "/images/products/cheesecake.jpg", IsAvailable = true }
            };

            await _context.Products.AddRangeAsync(products);
            await _context.SaveChangesAsync();

            return products.Count;
        }
    }
}
