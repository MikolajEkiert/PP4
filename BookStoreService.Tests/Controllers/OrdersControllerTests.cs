using BookStore_Application.Services;
using BookStore_Domain.Models;
using BookStoreService.Controllers;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace BookStoreService.Tests.Controllers;

public class OrdersControllerTests
{
    private readonly Mock<IOrder_Service> _orderServiceMock;
    private readonly OrdersController _controller;

    public OrdersControllerTests()
    {
        _orderServiceMock = new Mock<IOrder_Service>();
        _controller = new OrdersController(_orderServiceMock.Object);
    }

    [Fact]
    public async Task GetAllOrders_ShouldReturnOkResultWithOrders()
    {
        // Arrange
        var orders = new List<Order>
        {
            new() { Id = 1, OrderNumber = "ORD-1" },
            new() { Id = 2, OrderNumber = "ORD-2" }
        };

        _orderServiceMock.Setup(x => x.GetAllOrders())
            .ReturnsAsync(orders);

        // Act
        var result = await _controller.GetAllOrders();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnValue = Assert.IsAssignableFrom<IEnumerable<Order>>(okResult.Value);
        Assert.Equal(2, returnValue.Count());
    }

    [Fact]
    public async Task GetOrderById_WithValidId_ShouldReturnOkResult()
    {
        // Arrange
        var order = new Order { Id = 1, OrderNumber = "ORD-1" };
        _orderServiceMock.Setup(x => x.GetOrderById(1))
            .ReturnsAsync(order);

        // Act
        var result = await _controller.GetOrderById(1);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnValue = Assert.IsType<Order>(okResult.Value);
        Assert.Equal(1, returnValue.Id);
    }

    [Fact]
    public async Task GetOrderById_WithInvalidId_ShouldReturnNotFound()
    {
        // Arrange
        _orderServiceMock.Setup(x => x.GetOrderById(1))
            .ReturnsAsync((Order?)null);

        // Act
        var result = await _controller.GetOrderById(1);

        // Assert
        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public async Task CreateOrder_WithValidData_ShouldReturnCreatedResult()
    {
        // Arrange
        var request = new CreateOrderRequest
        {
            CustomerEmail = "test@example.com",
            ShippingAddress = "123 Test St",
            Items = new List<OrderItem>
            {
                new() { BookId = 1, Quantity = 2, UnitPrice = 29.99m, Subtotal = 59.98m }
            }
        };

        var order = new Order
        {
            Id = 1,
            OrderNumber = "ORD-1",
            CustomerEmail = request.CustomerEmail,
            ShippingAddress = request.ShippingAddress,
            OrderItems = request.Items,
            TotalAmount = 59.98m
        };

        _orderServiceMock.Setup(x => x.CreateOrder(
                request.CustomerEmail,
                request.ShippingAddress,
                request.Items))
            .ReturnsAsync(order);

        // Act
        var result = await _controller.CreateOrder(request);

        // Assert
        var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
        var returnValue = Assert.IsType<Order>(createdResult.Value);
        Assert.Equal(1, returnValue.Id);
        Assert.Equal("GetOrderById", createdResult.ActionName);
        Assert.Equal(1, createdResult.RouteValues?["id"]);
    }

    [Fact]
    public async Task CreateOrder_WithInvalidData_ShouldReturnBadRequest()
    {
        // Arrange
        var request = new CreateOrderRequest
        {
            CustomerEmail = "test@example.com",
            ShippingAddress = "123 Test St",
            Items = new List<OrderItem>
            {
                new() { BookId = 1, Quantity = 2, UnitPrice = 29.99m, Subtotal = 59.98m }
            }
        };

        _orderServiceMock.Setup(x => x.CreateOrder(
                request.CustomerEmail,
                request.ShippingAddress,
                request.Items))
            .ThrowsAsync(new InvalidOperationException("Insufficient stock"));

        // Act
        var result = await _controller.CreateOrder(request);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
        Assert.Equal("Insufficient stock", badRequestResult.Value);
    }

    [Fact]
    public async Task UpdateOrderStatus_WithValidData_ShouldReturnNoContent()
    {
        // Arrange
        var orderId = 1;
        var status = OrderStatus.Processing;

        _orderServiceMock.Setup(x => x.UpdateOrderStatus(orderId, status))
            .Returns((Task<Order>)Task.CompletedTask);

        // Act
        var result = await _controller.UpdateOrderStatus(orderId, status);

        // Assert
        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task UpdateOrderStatus_WithInvalidOrderId_ShouldReturnNotFound()
    {
        // Arrange
        var orderId = 1;
        var status = OrderStatus.Processing;

        _orderServiceMock.Setup(x => x.UpdateOrderStatus(orderId, status))
            .ThrowsAsync(new ArgumentException("Order not found"));

        // Act
        var result = await _controller.UpdateOrderStatus(orderId, status);

        // Assert
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
        Assert.Equal("Order not found", notFoundResult.Value);
    }
} 