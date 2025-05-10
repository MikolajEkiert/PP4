using BookStore_Application.Services;
using BookStore_Domain.Models;
using BookStore_Domain.Repositories;
using Moq;
using Xunit;

namespace BookStore_Application.Tests.Services;

public class Order_ServiceTests
{
    private readonly Mock<IOrder_Repository> _orderRepositoryMock;
    private readonly Mock<IBook_Repository> _bookRepositoryMock;
    private readonly Order_Service _orderService;

    public Order_ServiceTests()
    {
        _orderRepositoryMock = new Mock<IOrder_Repository>();
        _bookRepositoryMock = new Mock<IBook_Repository>();
        _orderService = new Order_Service(_orderRepositoryMock.Object, _bookRepositoryMock.Object);
    }

    [Fact]
    public async Task CreateOrder_WithValidData_ShouldCreateOrderSuccessfully()
    {
        // Arrange
        var customerEmail = "test@example.com";
        var shippingAddress = "123 Test St";
        var book = new Book { Id = 1, Title = "Test Book", Price = 29.99m, Stock = 10 };
        var items = new List<OrderItem>
        {
            new() { BookId = book.Id, Quantity = 2, UnitPrice = book.Price, Subtotal = book.Price * 2 }
        };

        _bookRepositoryMock.Setup(x => x.GetBookById(book.Id))
            .ReturnsAsync(book);

        _orderRepositoryMock.Setup(x => x.AddOrder(It.IsAny<Order>()))
            .ReturnsAsync((Order order) => order);

        // Act
        var result = await _orderService.CreateOrder(customerEmail, shippingAddress, items);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(customerEmail, result.CustomerEmail);
        Assert.Equal(shippingAddress, result.ShippingAddress);
        Assert.Equal(OrderStatus.Pending, result.Status);
        Assert.Single(result.OrderItems);
        Assert.Equal(59.98m, result.TotalAmount);
        Assert.True(result.OrderNumber.StartsWith("ORD-"));

        _bookRepositoryMock.Verify(x => x.UpdateBook(It.IsAny<Book>()), Times.Once);
        _orderRepositoryMock.Verify(x => x.AddOrder(It.IsAny<Order>()), Times.Once);
    }

    [Fact]
    public async Task CreateOrder_WithInsufficientStock_ShouldThrowException()
    {
        // Arrange
        var customerEmail = "test@example.com";
        var shippingAddress = "123 Test St";
        var book = new Book { Id = 1, Title = "Test Book", Price = 29.99m, Stock = 1 };
        var items = new List<OrderItem>
        {
            new() { BookId = book.Id, Quantity = 2, UnitPrice = book.Price, Subtotal = book.Price * 2 }
        };

        _bookRepositoryMock.Setup(x => x.GetBookById(book.Id))
            .ReturnsAsync(book);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            _orderService.CreateOrder(customerEmail, shippingAddress, items));

        _bookRepositoryMock.Verify(x => x.UpdateBook(It.IsAny<Book>()), Times.Never);
        _orderRepositoryMock.Verify(x => x.AddOrder(It.IsAny<Order>()), Times.Never);
    }

    [Fact]
    public async Task UpdateOrderStatus_WithValidData_ShouldUpdateStatus()
    {
        // Arrange
        var orderId = 1;
        var order = new Order
        {
            Id = orderId,
            OrderNumber = "ORD-20240101-12345678",
            Status = OrderStatus.Pending
        };

        _orderRepositoryMock.Setup(x => x.GetOrderById(orderId))
            .ReturnsAsync(order);

        _orderRepositoryMock.Setup(x => x.UpdateOrder(It.IsAny<Order>()))
            .ReturnsAsync((Order o) => o);

        // Act
        var result = await _orderService.UpdateOrderStatus(orderId, OrderStatus.Processing);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(OrderStatus.Processing, result.Status);
        _orderRepositoryMock.Verify(x => x.UpdateOrder(It.IsAny<Order>()), Times.Once);
    }

    [Fact]
    public async Task UpdateOrderStatus_WithInvalidOrderId_ShouldThrowException()
    {
        // Arrange
        var orderId = 1;
        _orderRepositoryMock.Setup(x => x.GetOrderById(orderId))
            .ReturnsAsync((Order?)null);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() =>
            _orderService.UpdateOrderStatus(orderId, OrderStatus.Processing));

        _orderRepositoryMock.Verify(x => x.UpdateOrder(It.IsAny<Order>()), Times.Never);
    }
} 