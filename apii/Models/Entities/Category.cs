namespace apii.Models.Entities;

/// <summary>
/// Entity Category - Danh mục sản phẩm
/// </summary>
public class Category
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public int DisplayOrder { get; set; } = 0;
    public bool IsActive { get; set; } = true;
    public DateTime CreatedDate { get; set; } = DateTime.Now;

    // Navigation properties
    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}
