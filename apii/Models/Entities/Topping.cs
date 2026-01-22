namespace apii.Models.Entities;

/// <summary>
/// Entity Topping - Topping cho sản phẩm
/// </summary>
public class Topping
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public bool IsAvailable { get; set; } = true;
}
