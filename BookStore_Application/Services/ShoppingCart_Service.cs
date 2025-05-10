using BookStore_Domain.Models;
using BookStore_Domain.Repositories;

namespace BookStore_Application.Services;

public class ShoppingCart_Service : IShoppingCart_Service
{
    private readonly IShoppingCart_Repository _shoppingCartRepository;
    private readonly IBook_Repository _bookRepository;
    private readonly IOrder_Service _orderService;

    public ShoppingCart_Service(
        IShoppingCart_Repository shoppingCartRepository,
        IBook_Repository bookRepository,
        IOrder_Service orderService)
    {
        _shoppingCartRepository = shoppingCartRepository;
        _bookRepository = bookRepository;
        _orderService = orderService;
    }

    public async Task<IEnumerable<ShoppingCart>> GetAllShoppingCarts()
    {
        return await _shoppingCartRepository.GetAllShoppingCarts();
    }

    public async Task<ShoppingCart?> GetShoppingCartById(int id)
    {
        return await _shoppingCartRepository.GetShoppingCartById(id);
    }

    public async Task<ShoppingCart?> GetShoppingCartByCustomerEmail(string email)
    {
        return await _shoppingCartRepository.GetShoppingCartByCustomerEmail(email);
    }

    public async Task<ShoppingCart> CreateShoppingCart(string customerEmail)
    {
        var existingCart = await _shoppingCartRepository.GetShoppingCartByCustomerEmail(customerEmail);
        if (existingCart != null)
            throw new InvalidOperationException("Shopping cart already exists for this customer");

        var shoppingCart = new ShoppingCart
        {
            CustomerEmail = customerEmail,
            CartItems = new List<CartItem>(),
            TotalAmount = 0
        };

        return await _shoppingCartRepository.AddShoppingCart(shoppingCart);
    }

    public async Task<ShoppingCart> UpdateShoppingCart(ShoppingCart shoppingCart)
    {
        var existingCart = await _shoppingCartRepository.GetShoppingCartById(shoppingCart.Id);
        if (existingCart == null)
            throw new ArgumentException("Shopping cart not found");

        return await _shoppingCartRepository.UpdateShoppingCart(shoppingCart);
    }

    public async Task DeleteShoppingCart(int id)
    {
        await _shoppingCartRepository.DeleteShoppingCart(id);
    }

    public async Task<CartItem> AddItemToCart(int shoppingCartId, int bookId, int quantity)
    {
        var shoppingCart = await _shoppingCartRepository.GetShoppingCartById(shoppingCartId);
        if (shoppingCart == null)
            throw new ArgumentException("Shopping cart not found");

        var book = await _bookRepository.GetBookById(bookId);
        if (book == null)
            throw new ArgumentException("Book not found");

        if (book.Stock.ToString().Length < quantity)
            throw new InvalidOperationException("Insufficient stock");

        var existingItem = shoppingCart.CartItems.FirstOrDefault(i => i.BookId == bookId);
        if (existingItem != null)
        {
            existingItem.Quantity += quantity;
            existingItem.Subtotal = existingItem.Quantity * existingItem.UnitPrice;
            return await _shoppingCartRepository.UpdateCartItem(existingItem);
        }

        var cartItem = new CartItem
        {
            ShoppingCartId = shoppingCartId,
            BookId = bookId,
            Quantity = quantity,
            UnitPrice = book.Price,
            Subtotal = book.Price * quantity
        };

        return await _shoppingCartRepository.AddCartItem(cartItem);
    }

    public async Task<CartItem> UpdateCartItemQuantity(int cartItemId, int quantity)
    {
        var cartItem = await _shoppingCartRepository.GetCartItemById(cartItemId);
        if (cartItem == null)
            throw new ArgumentException("Cart item not found");

        var book = await _bookRepository.GetBookById(cartItem.BookId);
        if (book == null)
            throw new ArgumentException("Book not found");

        if (book.Stock.ToString().Length < quantity)
            throw new InvalidOperationException("Insufficient stock");

        cartItem.Quantity = quantity;
        cartItem.Subtotal = quantity * cartItem.UnitPrice;

        return await _shoppingCartRepository.UpdateCartItem(cartItem);
    }

    public async Task RemoveItemFromCart(int cartItemId)
    {
        await _shoppingCartRepository.DeleteCartItem(cartItemId);
    }

    public async Task<Order> Checkout(int shoppingCartId, string shippingAddress)
    {
        var shoppingCart = await _shoppingCartRepository.GetShoppingCartById(shoppingCartId);
        if (shoppingCart == null)
            throw new ArgumentException("Shopping cart not found");

        if (!shoppingCart.CartItems.Any())
            throw new InvalidOperationException("Shopping cart is empty");

        var orderItems = shoppingCart.CartItems.Select(item => new OrderItem
        {
            BookId = item.BookId,
            Quantity = item.Quantity,
            UnitPrice = item.UnitPrice,
            Subtotal = item.Subtotal
        }).ToList();

        var order = await _orderService.CreateOrder(
            shoppingCart.CustomerEmail,
            shippingAddress,
            orderItems
        );

        await _shoppingCartRepository.DeleteShoppingCart(shoppingCartId);

        return order;
    }
} 