using BookEShop.Application.Interfaces;
using Xunit;
using Moq;
using FluentAssertions;
using BookEShop.Application.Services;
using BookEShop.Domain.Models;
using BookEShop.Domain.Repositories;
using BookEShop.Domain.Enums;

namespace BookEShop.Test.Unit.Services;

public class OrderServiceTests
{
    private readonly Mock<IOrderRepository> _orderRepositoryMock;
    private readonly Mock<IBookRepository> _bookRepositoryMock;
    private readonly IOrderService _orderService;

    public OrderServiceTests()
    {
        _orderRepositoryMock = new Mock<IOrderRepository>();
        _bookRepositoryMock = new Mock<IBookRepository>();
        _orderService = new OrderService(_orderRepositoryMock.Object, _bookRepositoryMock.Object);
    }

    [Fact]
    public async Task GetAllOrders_ShouldReturnAllOrders()
    {
        // Arrange
        var expectedOrders = new List<Order>
        {
            new() { Id = 1, OrderNumber = "ORD-20240101-12345678", CustomerEmail = "test1@example.com", Status = OrderStatus.Pending },
            new() { Id = 2, OrderNumber = "ORD-20240101-87654321", CustomerEmail = "test2@example.com", Status = OrderStatus.Delivered }
        };
        _orderRepositoryMock.Setup(repo => repo.GetAllOrders())
            .ReturnsAsync(expectedOrders);

        // Act
        var result = await _orderService.GetAllOrders();

        // Assert
        result.Should().BeEquivalentTo(expectedOrders);
        _orderRepositoryMock.Verify(repo => repo.GetAllOrders(), Times.Once);
    }

    [Fact]
    public async Task GetOrderById_WhenOrderExists_ShouldReturnOrder()
    {
        // Arrange
        var expectedOrder = new Order 
        { 
            Id = 1, 
            OrderNumber = "ORD-20240101-12345678", 
            CustomerEmail = "test@example.com", 
            Status = OrderStatus.Pending 
        };
        _orderRepositoryMock.Setup(repo => repo.GetOrderById(1))
            .ReturnsAsync(expectedOrder);

        // Act
        var result = await _orderService.GetOrderById(1);

        // Assert
        result.Should().BeEquivalentTo(expectedOrder);
        _orderRepositoryMock.Verify(repo => repo.GetOrderById(1), Times.Once);
    }

    [Fact]
    public async Task GetOrderByOrderNumber_WhenOrderExists_ShouldReturnOrder()
    {
        // Arrange
        var expectedOrder = new Order 
        { 
            Id = 1, 
            OrderNumber = "ORD-20240101-12345678", 
            CustomerEmail = "test@example.com", 
            Status = OrderStatus.Pending 
        };
        _orderRepositoryMock.Setup(repo => repo.GetOrderByOrderNumber("ORD-20240101-12345678"))
            .ReturnsAsync(expectedOrder);

        // Act
        var result = await _orderService.GetOrderByOrderNumber("ORD-20240101-12345678");

        // Assert
        result.Should().BeEquivalentTo(expectedOrder);
        _orderRepositoryMock.Verify(repo => repo.GetOrderByOrderNumber("ORD-20240101-12345678"), Times.Once);
    }

    [Fact]
    public async Task CreateOrder_WithValidItems_ShouldCreateOrder()
    {
        // Arrange
        var customerEmail = "test@example.com";
        var shippingAddress = "123 Test St";
        var items = new List<OrderItem>
        {
            new() { BookId = 1, Quantity = 2, UnitPrice = 29.99m, Subtotal = 59.98m },
            new() { BookId = 2, Quantity = 1, UnitPrice = 39.99m, Subtotal = 39.99m }
        };

        var books = new Dictionary<int, Book>
        {
            { 1, new Book { Id = 1, Stock = 5 } },
            { 2, new Book { Id = 2, Stock = 3 } }
        };

        _bookRepositoryMock.Setup(repo => repo.GetBookById(It.IsAny<int>()))
            .ReturnsAsync((int id) => books[id]);

        _orderRepositoryMock.Setup(repo => repo.AddOrder(It.IsAny<Order>()))
            .ReturnsAsync((Order order) => order);

        // Act
        var result = await _orderService.CreateOrder(customerEmail, shippingAddress, items);

        // Assert
        result.Should().NotBeNull();
        result.CustomerEmail.Should().Be(customerEmail);
        result.ShippingAddress.Should().Be(shippingAddress);
        result.Status.Should().Be(OrderStatus.Pending);
        result.OrderItems.Should().BeEquivalentTo(items);
        result.TotalAmount.Should().Be(items.Sum(i => i.Subtotal));
        result.OrderNumber.Should().Match("ORD-*");

        _bookRepositoryMock.Verify(repo => repo.GetBookById(1), Times.Once);
        _bookRepositoryMock.Verify(repo => repo.GetBookById(2), Times.Once);
        _orderRepositoryMock.Verify(repo => repo.AddOrder(It.IsAny<Order>()), Times.Once);
    }

    [Fact]
    public async Task CreateOrder_WithNonExistentBook_ShouldThrowArgumentException()
    {
        // Arrange
        var customerEmail = "test@example.com";
        var shippingAddress = "123 Test St";
        var items = new List<OrderItem>
        {
            new() { BookId = 999, Quantity = 1, UnitPrice = 29.99m, Subtotal = 29.99m }
        };

        _bookRepositoryMock.Setup(repo => repo.GetBookById(999))
            .ReturnsAsync((Book?)null);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => 
            _orderService.CreateOrder(customerEmail, shippingAddress, items));
    }

    [Fact]
    public async Task CreateOrder_WithInsufficientStock_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var customerEmail = "test@example.com";
        var shippingAddress = "123 Test St";
        var items = new List<OrderItem>
        {
            new() { BookId = 1, Quantity = 10, UnitPrice = 29.99m, Subtotal = 299.90m }
        };

        var book = new Book { Id = 1, Stock = 5 };
        _bookRepositoryMock.Setup(repo => repo.GetBookById(1))
            .ReturnsAsync(book);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => 
            _orderService.CreateOrder(customerEmail, shippingAddress, items));
    }

    [Fact]
    public async Task UpdateOrderStatus_WhenOrderExists_ShouldUpdateStatus()
    {
        // Arrange
        var order = new Order 
        { 
            Id = 1, 
            OrderNumber = "ORD-20240101-12345678", 
            CustomerEmail = "test@example.com", 
            Status = OrderStatus.Pending 
        };

        _orderRepositoryMock.Setup(repo => repo.GetOrderById(1))
            .ReturnsAsync(order);
        _orderRepositoryMock.Setup(repo => repo.UpdateOrder(It.IsAny<Order>()))
            .ReturnsAsync((Order o) => o);

        // Act
        var result = await _orderService.UpdateOrderStatus(1, OrderStatus.Delivered);

        // Assert
        result.Status.Should().Be(OrderStatus.Delivered);
        _orderRepositoryMock.Verify(repo => repo.GetOrderById(1), Times.Once);
        _orderRepositoryMock.Verify(repo => repo.UpdateOrder(It.IsAny<Order>()), Times.Once);
    }

    [Fact]
    public async Task UpdateOrderStatus_WhenOrderDoesNotExist_ShouldThrowArgumentException()
    {
        // Arrange
        _orderRepositoryMock.Setup(repo => repo.GetOrderById(999))
            .ReturnsAsync((Order?)null);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => 
            _orderService.UpdateOrderStatus(999, OrderStatus.Delivered));
    }
} 