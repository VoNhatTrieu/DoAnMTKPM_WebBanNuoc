namespace apii.Models.Entities;

/// <summary>
/// Entity User - Người dùng
/// </summary>
public class User
{
    public int Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    public string PasswordHash { get; set; } = string.Empty;
    public string Role { get; set; } = "Customer"; // Customer, Admin
    public bool IsActive { get; set; } = true;
    public DateTime CreatedDate { get; set; } = DateTime.Now;
    public DateTime? LastLoginDate { get; set; }

    // Navigation properties
    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
    public virtual ICollection<Cart> Carts { get; set; } = new List<Cart>();
    
    /// <summary>
    /// Products owned by this user - for data segregation
    /// </summary>
    public virtual ICollection<Product> OwnedProducts { get; set; } = new List<Product>();
}
