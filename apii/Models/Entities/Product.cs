namespace apii.Models.Entities;

/// <summary>
/// Entity Product - Sản phẩm
/// </summary>
public class Product
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal BasePrice { get; set; }
    public int CategoryId { get; set; }
    public string? ImageUrl { get; set; }
    public bool IsAvailable { get; set; } = true;
    public DateTime CreatedDate { get; set; } = DateTime.Now;

    // Navigation properties
    public virtual Category Category { get; set; } = null!;
}
