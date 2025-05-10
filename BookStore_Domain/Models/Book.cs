using BookStore_Domain.Models;

namespace BookStore_Domain.Models;

public class Book : Base_Model
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string ISBN { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int Stock { get; set; } = 0;
    public string Sku { get; set; } = string.Empty;
    public Category Book_Category { get; set; } = default!;
    public string Author { get; set; } = string.Empty;
    public string Publisher { get; set; } = string.Empty;
    public int PublicationYear { get; set; }
} 