using BookEShop.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookEShop.Domain.Repositories;

public interface IShoppingCartRepository
{
    Task<IEnumerable<ShoppingCart>> GetAllShoppingCarts();
    Task<ShoppingCart?> GetShoppingCartById(int id);
    Task<ShoppingCart?> GetShoppingCartByCustomerEmail(string email);
    Task<ShoppingCart> AddShoppingCart(ShoppingCart shoppingCart);
    Task<ShoppingCart> UpdateShoppingCart(ShoppingCart shoppingCart);
    Task DeleteShoppingCart(int id);
    Task<CartItem?> GetCartItemById(int id);
    Task<CartItem> AddCartItem(CartItem cartItem);
    Task<CartItem> UpdateCartItem(CartItem cartItem);
    Task DeleteCartItem(int id);
}