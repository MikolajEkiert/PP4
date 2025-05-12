using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BookEShop.Domain.Models;

namespace BookEShop.Domain.Models;

public class Book : BaseModel
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string ISBN { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int Stock { get; set; } = 0;
    public string Sku { get; set; } = string.Empty;
    public Category BookCategory { get; set; } = default!;
    public string Author { get; set; } = string.Empty;
    public int PublicationYear { get; set; }
}

