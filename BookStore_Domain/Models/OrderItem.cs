using BookStore_Domain.Models;

namespace BookStore_Domain.Models;

public class OrderItem : Base_Model
{
    public int Id { get; set; }
    public int OrderId { get; set; }
    public Order Order { get; set; } = default!;
    public int BookId { get; set; }
    public Book Book { get; set; } = default!;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal Subtotal { get; set; }
} 