using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BookEShop.Domain.Models;

namespace BookEShop.Domain.Models;

public class ShoppingCart : BaseModel
{
    public int Id { get; set; }
    public string CustomerEmail { get; set; } = string.Empty;
    public List<CartItem> CartItems { get; set; } = new();
    public decimal TotalAmount { get; set; }
}
