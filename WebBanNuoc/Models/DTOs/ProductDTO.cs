namespace WebBanNuoc.Models.DTOs
{
    /// <summary>
    /// DTO Pattern - Data Transfer Object
    /// Tách Entity khỏi dữ liệu Request/Response
    /// </summary>
    public class ProductDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal BasePrice { get; set; }
        public string CategoryName { get; set; }
        public string ImageUrl { get; set; }
        public int ReviewCount { get; set; }
        public decimal AverageRating { get; set; }
    }
}
