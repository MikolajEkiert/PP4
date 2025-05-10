using BookStore_Domain.Models;

namespace BookStore_Domain.Repositories;

public interface IShoppingCart_Repository
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