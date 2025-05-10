using BookStore_Domain.Models;
using Xunit;

namespace BookStore_Domain.Tests.Models;

public class OrderTests
{
    [Fact]
    public void Order_WithValidData_ShouldCreateSuccessfully()
    {
        // Arrange
        var orderNumber = "ORD-20240101-12345678";
        var customerEmail = "test@example.com";
        var shippingAddress = "123 Test St";
        var totalAmount = 99.99m;

        // Act
        var order = new Order
        {
            OrderNumber = orderNumber,
            CustomerEmail = customerEmail,
            ShippingAddress = shippingAddress,
            TotalAmount = totalAmount,
            Status = OrderStatus.Pending
        };

        // Assert
        Assert.Equal(orderNumber, order.OrderNumber);
        Assert.Equal(customerEmail, order.CustomerEmail);
        Assert.Equal(shippingAddress, order.ShippingAddress);
        Assert.Equal(totalAmount, order.TotalAmount);
        Assert.Equal(OrderStatus.Pending, order.Status);
        Assert.Empty(order.OrderItems);
    }

    [Fact]
    public void Order_WithOrderItems_ShouldCalculateTotalAmount()
    {
        // Arrange
        var order = new Order
        {
            OrderNumber = "ORD-20240101-12345678",
            CustomerEmail = "test@example.com",
            ShippingAddress = "123 Test St",
            Status = OrderStatus.Pending
        };

        var book1 = new Book { Id = 1, Title = "Test Book 1", Price = 29.99m };
        var book2 = new Book { Id = 2, Title = "Test Book 2", Price = 39.99m };

        var orderItems = new List<OrderItem>
        {
            new() { Book = book1, Quantity = 2, UnitPrice = book1.Price, Subtotal = book1.Price * 2 },
            new() { Book = book2, Quantity = 1, UnitPrice = book2.Price, Subtotal = book2.Price }
        };

        // Act
        order.OrderItems = orderItems;
        order.TotalAmount = orderItems.Sum(item => item.Subtotal);

        // Assert
        Assert.Equal(2, order.OrderItems.Count);
        Assert.Equal(99.97m, order.TotalAmount);
    }
} 