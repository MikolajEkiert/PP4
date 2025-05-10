namespace BookStore_Domain.Models;

public class Category : Base_Model
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
} 