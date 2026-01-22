namespace apii.Models.Entities;

/// <summary>
/// Entity Size - Size sản phẩm
/// </summary>
public class Size
{
    public int Id { get; set; }
    public string Code { get; set; } = string.Empty; // S, M, L
    public string Name { get; set; } = string.Empty;
    public decimal AdditionalPrice { get; set; } = 0;
}
