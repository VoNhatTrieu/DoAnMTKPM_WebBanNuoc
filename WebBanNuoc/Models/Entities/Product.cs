using System;
using System.Collections.Generic;

namespace WebBanNuoc.Models.Entities
{
    /// <summary>
    /// Entity Product - Sản phẩm
    /// </summary>
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal BasePrice { get; set; }
        public string CategoryId { get; set; }
        public string ImageUrl { get; set; }
        public bool IsAvailable { get; set; }
        public DateTime CreatedDate { get; set; }
    }

    /// <summary>
    /// Size Configuration
    /// </summary>
    public class Size
    {
        public string Code { get; set; } // S, M, L
        public string Name { get; set; }
        public decimal AdditionalPrice { get; set; }
    }

    /// <summary>
    /// Topping Configuration
    /// </summary>
    public class Topping
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
    }
}
