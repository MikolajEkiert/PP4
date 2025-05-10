using BookStore_Domain.Models;

namespace BookStore_Domain.Models;

public class ShoppingCart : Base_Model
{
    public int Id { get; set; }
    public string CustomerEmail { get; set; } = string.Empty;
    public List<CartItem> CartItems { get; set; } = new();
    public decimal TotalAmount { get; set; }
} 