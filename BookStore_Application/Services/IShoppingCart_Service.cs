using BookStore_Domain.Models;

namespace BookStore_Application.Services;

public interface IShoppingCart_Service
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