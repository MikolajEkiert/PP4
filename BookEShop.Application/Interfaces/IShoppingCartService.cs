using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BookEShop.Domain.Models;

namespace BookEShop.Application.Interfaces;

public interface IShoppingCartService
{
    Task<IEnumerable<ShoppingCart>> GetAllShoppingCarts();
    Task<ShoppingCart?> GetShoppingCartById(int id);
    Task<ShoppingCart?> GetShoppingCartByCustomerEmail(string email);
    Task<ShoppingCart> CreateShoppingCart(string customerEmail);
    Task<ShoppingCart> UpdateShoppingCart(ShoppingCart shoppingCart);
    Task DeleteShoppingCart(int id);
    Task<CartItem> AddItemToCart(int shoppingCartId, int bookId, int quantity);
    Task<CartItem> UpdateCartItemQuantity(int cartItemId, int quantity);
    Task RemoveItemFromCart(int cartItemId);
    Task<Order> Checkout(int shoppingCartId, string shippingAddress);
}