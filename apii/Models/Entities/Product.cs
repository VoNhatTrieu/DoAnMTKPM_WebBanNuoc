using apii.Models;

namespace apii.Models.Entities;

/// <summary>
/// Entity Product - Sản phẩm
/// </summary>
public class Product : IOwnable
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal BasePrice { get; set; }
    public int CategoryId { get; set; }
    public string? ImageUrl { get; set; }
    public bool IsAvailable { get; set; } = true;
    public DateTime CreatedDate { get; set; } = DateTime.Now;
    
    /// <summary>
    /// Owner ID for data segregation - Each product belongs to a specific user
    /// </summary>
    public int OwnerId { get; set; }

    // Navigation properties
    public virtual Category Category { get; set; } = null!;
    
    /// <summary>
    /// Owner/Creator of this product - for ownership-based access control
    /// </summary>
    public virtual User Owner { get; set; } = null!;
}
