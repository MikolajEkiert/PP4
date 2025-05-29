using Xunit;
using Moq;
using FluentAssertions;
using BookEShop.Application.Services;
using BookEShop.Domain.Models;
using BookEShop.Domain.Repositories;
using BookEShop.Application.Interfaces;

namespace BookEShop.Test.Unit.Services;

public class ShoppingCartServiceTests
{
    private readonly Mock<IShoppingCartRepository> _shoppingCartRepositoryMock;
    private readonly Mock<IBookRepository> _bookRepositoryMock;
    private readonly Mock<IOrderService> _orderServiceMock;
    private readonly IShoppingCartService _shoppingCartService;

    public ShoppingCartServiceTests()
    {
        _shoppingCartRepositoryMock = new Mock<IShoppingCartRepository>();
        _bookRepositoryMock = new Mock<IBookRepository>();
        _orderServiceMock = new Mock<IOrderService>();
        _shoppingCartService = new ShoppingCartService(
            _shoppingCartRepositoryMock.Object,
            _bookRepositoryMock.Object,
            _orderServiceMock.Object);
    }

    [Fact]
    public async Task GetAllShoppingCarts_ShouldReturnAllCarts()
    {
        // Arrange
        var expectedCarts = new List<ShoppingCart>
        {
            new() { Id = 1, CustomerEmail = "test1@example.com", TotalAmount = 29.99m },
            new() { Id = 2, CustomerEmail = "test2@example.com", TotalAmount = 39.99m }
        };
        _shoppingCartRepositoryMock.Setup(repo => repo.GetAllShoppingCarts())
            .ReturnsAsync(expectedCarts);

        // Act
        var result = await _shoppingCartService.GetAllShoppingCarts();

        // Assert
        result.Should().BeEquivalentTo(expectedCarts);
        _shoppingCartRepositoryMock.Verify(repo => repo.GetAllShoppingCarts(), Times.Once);
    }

    [Fact]
    public async Task GetShoppingCartById_WhenCartExists_ShouldReturnCart()
    {
        // Arrange
        var expectedCart = new ShoppingCart 
        { 
            Id = 1, 
            CustomerEmail = "test@example.com", 
            TotalAmount = 29.99m 
        };
        _shoppingCartRepositoryMock.Setup(repo => repo.GetShoppingCartById(1))
            .ReturnsAsync(expectedCart);

        // Act
        var result = await _shoppingCartService.GetShoppingCartById(1);

        // Assert
        result.Should().BeEquivalentTo(expectedCart);
        _shoppingCartRepositoryMock.Verify(repo => repo.GetShoppingCartById(1), Times.Once);
    }

    [Fact]
    public async Task CreateShoppingCart_WhenCustomerHasNoCart_ShouldCreateCart()
    {
        // Arrange
        var customerEmail = "test@example.com";
        _shoppingCartRepositoryMock.Setup(repo => repo.GetShoppingCartByCustomerEmail(customerEmail))
            .ReturnsAsync((ShoppingCart?)null);
        _shoppingCartRepositoryMock.Setup(repo => repo.AddShoppingCart(It.IsAny<ShoppingCart>()))
            .ReturnsAsync((ShoppingCart cart) => cart);

        // Act
        var result = await _shoppingCartService.CreateShoppingCart(customerEmail);

        // Assert
        result.Should().NotBeNull();
        result.CustomerEmail.Should().Be(customerEmail);
        result.TotalAmount.Should().Be(0);
        result.CartItems.Should().BeEmpty();

        _shoppingCartRepositoryMock.Verify(repo => repo.GetShoppingCartByCustomerEmail(customerEmail), Times.Once);
        _shoppingCartRepositoryMock.Verify(repo => repo.AddShoppingCart(It.IsAny<ShoppingCart>()), Times.Once);
    }

    [Fact]
    public async Task CreateShoppingCart_WhenCustomerHasExistingCart_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var customerEmail = "test@example.com";
        var existingCart = new ShoppingCart { Id = 1, CustomerEmail = customerEmail };
        _shoppingCartRepositoryMock.Setup(repo => repo.GetShoppingCartByCustomerEmail(customerEmail))
            .ReturnsAsync(existingCart);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => 
            _shoppingCartService.CreateShoppingCart(customerEmail));
    }

    [Fact]
    public async Task AddItemToCart_WithValidBookAndQuantity_ShouldAddItem()
    {
        // Arrange
        var cartId = 1;
        var bookId = 1;
        var quantity = 2;
        var book = new Book { Id = bookId, Price = 29.99m, Stock = 5 };
        var cart = new ShoppingCart { Id = cartId, CartItems = new List<CartItem>() };

        _shoppingCartRepositoryMock.Setup(repo => repo.GetShoppingCartById(cartId))
            .ReturnsAsync(cart);
        _bookRepositoryMock.Setup(repo => repo.GetBookById(bookId))
            .ReturnsAsync(book);
        _shoppingCartRepositoryMock.Setup(repo => repo.AddCartItem(It.IsAny<CartItem>()))
            .ReturnsAsync((CartItem item) => item);

        // Act
        var result = await _shoppingCartService.AddItemToCart(cartId, bookId, quantity);

        // Assert
        result.Should().NotBeNull();
        result.ShoppingCartId.Should().Be(cartId);
        result.BookId.Should().Be(bookId);
        result.Quantity.Should().Be(quantity);
        result.UnitPrice.Should().Be(book.Price);
        result.Subtotal.Should().Be(book.Price * quantity);

        _shoppingCartRepositoryMock.Verify(repo => repo.GetShoppingCartById(cartId), Times.Once);
        _bookRepositoryMock.Verify(repo => repo.GetBookById(bookId), Times.Once);
        _shoppingCartRepositoryMock.Verify(repo => repo.AddCartItem(It.IsAny<CartItem>()), Times.Once);
    }

    [Fact]
    public async Task AddItemToCart_WithNonExistentCart_ShouldThrowArgumentException()
    {
        // Arrange
        _shoppingCartRepositoryMock.Setup(repo => repo.GetShoppingCartById(999))
            .ReturnsAsync((ShoppingCart?)null);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => 
            _shoppingCartService.AddItemToCart(999, 1, 1));
    }

    [Fact]
    public async Task AddItemToCart_WithNonExistentBook_ShouldThrowArgumentException()
    {
        // Arrange
        var cart = new ShoppingCart { Id = 1 };
        _shoppingCartRepositoryMock.Setup(repo => repo.GetShoppingCartById(1))
            .ReturnsAsync(cart);
        _bookRepositoryMock.Setup(repo => repo.GetBookById(999))
            .ReturnsAsync((Book?)null);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => 
            _shoppingCartService.AddItemToCart(1, 999, 1));
    }

    [Fact]
    public async Task AddItemToCart_WithInsufficientStock_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var cart = new ShoppingCart { Id = 1 };
        var book = new Book { Id = 1, Stock = 5 };
        _shoppingCartRepositoryMock.Setup(repo => repo.GetShoppingCartById(1))
            .ReturnsAsync(cart);
        _bookRepositoryMock.Setup(repo => repo.GetBookById(1))
            .ReturnsAsync(book);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => 
            _shoppingCartService.AddItemToCart(1, 1, 10));
    }

    [Fact]
    public async Task Checkout_WithValidCart_ShouldCreateOrderAndDeleteCart()
    {
        // Arrange
        var cartId = 1;
        var shippingAddress = "123 Test St";
        var cart = new ShoppingCart
        {
            Id = cartId,
            CustomerEmail = "test@example.com",
            CartItems = new List<CartItem>
            {
                new() { BookId = 1, Quantity = 2, UnitPrice = 29.99m, Subtotal = 59.98m }
            }
        };

        var expectedOrder = new Order
        {
            Id = 1,
            OrderNumber = "ORD-20240101-12345678",
            CustomerEmail = cart.CustomerEmail,
            ShippingAddress = shippingAddress,
            Status = Domain.Enums.OrderStatus.Pending
        };

        _shoppingCartRepositoryMock.Setup(repo => repo.GetShoppingCartById(cartId))
            .ReturnsAsync(cart);
        _orderServiceMock.Setup(service => service.CreateOrder(
                cart.CustomerEmail,
                shippingAddress,
                It.IsAny<List<OrderItem>>()))
            .ReturnsAsync(expectedOrder);

        // Act
        var result = await _shoppingCartService.Checkout(cartId, shippingAddress);

        // Assert
        result.Should().BeEquivalentTo(expectedOrder);
        _shoppingCartRepositoryMock.Verify(repo => repo.GetShoppingCartById(cartId), Times.Once);
        _orderServiceMock.Verify(service => service.CreateOrder(
            cart.CustomerEmail,
            shippingAddress,
            It.IsAny<List<OrderItem>>()), Times.Once);
        _shoppingCartRepositoryMock.Verify(repo => repo.DeleteShoppingCart(cartId), Times.Once);
    }

    [Fact]
    public async Task Checkout_WithEmptyCart_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var cart = new ShoppingCart
        {
            Id = 1,
            CustomerEmail = "test@example.com",
            CartItems = new List<CartItem>()
        };

        _shoppingCartRepositoryMock.Setup(repo => repo.GetShoppingCartById(1))
            .ReturnsAsync(cart);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => 
            _shoppingCartService.Checkout(1, "123 Test St"));
    }
} 