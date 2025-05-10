using BookStore_Domain.Models;

namespace BookStore_Domain.Models;

public class CartItem : Base_Model
{
    public int Id { get; set; }
    public int ShoppingCartId { get; set; }
    public ShoppingCart ShoppingCart { get; set; } = default!;
    public int BookId { get; set; }
    public Book Book { get; set; } = default!;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal Subtotal { get; set; }
} 