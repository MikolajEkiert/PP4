using System.Net;
using System.Text;
using System.Text.Json;
using BookEShop.Controllers;
using BookEShop.Domain.Models;
using BookEShop.Domain.Enums;
using BookEShop.Test.Integration;
using FluentAssertions;
using Newtonsoft.Json;
using Xunit;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace BookEShop.Test.Integration.Controllers;

public class OrdersControllerTests : TestBase
{
    [Fact]
    public async Task GetAllOrders_ShouldReturnAllOrders()
    {
        // Act
        var response = await Client.GetAsync("/api/Orders");
        var content = await response.Content.ReadAsStringAsync();
        var orders = JsonSerializer.Deserialize<List<Order>>(content, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        orders.Should().NotBeNull();
    }

    [Fact]
    public async Task GetOrderById_WhenOrderExists_ShouldReturnOrder()
    {
        // Arrange
        var order = new Order
        {
            OrderNumber = "ORD-20240101-12345678",
            CustomerEmail = "test@example.com",
            ShippingAddress = "123 Test St",
            Status = OrderStatus.Pending,
            TotalAmount = 29.99m
        };
        Context.Orders.Add(order);
        await Context.SaveChangesAsync();

        // Act
        var response = await Client.GetAsync($"/api/Orders/{order.Id}");
        var content = await response.Content.ReadAsStringAsync();
        var returnedOrder = JsonSerializer.Deserialize<Order>(content, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        returnedOrder.Should().NotBeNull();
        returnedOrder!.Id.Should().Be(order.Id);
        returnedOrder.OrderNumber.Should().Be(order.OrderNumber);
    }

    [Fact]
    public async Task GetOrderById_WhenOrderDoesNotExist_ShouldReturnNotFound()
    {
        // Act
        var response = await Client.GetAsync("/api/Orders/999");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task CreateOrder_WithInsufficientStock_ShouldReturnBadRequest()
    {
        // Arrange
        var book = new Book
        {
            Title = "Test Book",
            ISBN = "1234567890",
            Price = 29.99m,
            Stock = 5
        };
        Context.Books.Add(book);
        await Context.SaveChangesAsync();

        var request = new
        {
            CustomerEmail = "test@example.com",
            ShippingAddress = "123 Test St",
            Items = new List<OrderItem>
            {
                new()
                {
                    BookId = book.Id,
                    Quantity = 10,
                    UnitPrice = book.Price,
                    Subtotal = book.Price * 10
                }
            }
        };

        var content = new StringContent(
            JsonSerializer.Serialize(request),
            Encoding.UTF8,
            "application/json");

        // Act
        var response = await Client.PostAsync("/api/Orders", content);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task UpdateOrderStatus_WhenOrderExists_ShouldUpdateStatus()
    {
        // Arrange
        var order = new Order
        {
            OrderNumber = "ORD-20240101-12345678",
            CustomerEmail = "test@example.com",
            ShippingAddress = "123 Test St",
            Status = OrderStatus.Pending,
            TotalAmount = 29.99m
        };
        Context.Orders.Add(order);
        await Context.SaveChangesAsync();

        var content = new StringContent(
            JsonSerializer.Serialize(OrderStatus.Cancelled),
            Encoding.UTF8,
            "application/json");

        // Act
        var response = await Client.PutAsync($"/api/Orders/{order.Id}/status", content);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var updatedOrder = await Context.Orders.FindAsync(order.Id);
        updatedOrder.Should().NotBeNull();
        updatedOrder!.Status.Should().NotBe(OrderStatus.Delivered);
    }

    [Fact]
    public async Task UpdateOrderStatus_WhenOrderDoesNotExist_ShouldReturnNotFound()
    {
        // Arrange
        var content = new StringContent(
            JsonSerializer.Serialize(OrderStatus.Delivered),
            Encoding.UTF8,
            "application/json");

        // Act
        var response = await Client.PutAsync("/api/Orders/999/status", content);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetOrdersByCustomerEmail_ShouldReturnCustomerOrders()
    {
        // Arrange
        var customerEmail = "test@example.com";
        var orders = new List<Order>
        {
            new()
            {
                OrderNumber = "ORD-20240101-12345678",
                CustomerEmail = customerEmail,
                ShippingAddress = "123 Test St",
                Status = OrderStatus.Pending,
                TotalAmount = 29.99m
            },
            new()
            {
                OrderNumber = "ORD-20240101-87654321",
                CustomerEmail = customerEmail,
                ShippingAddress = "456 Test St",
                Status = OrderStatus.Delivered,
                TotalAmount = 39.99m
            }
        };
        Context.Orders.AddRange(orders);
        await Context.SaveChangesAsync();

        // Act
        var response = await Client.GetAsync($"/api/Orders/customer/{customerEmail}");
        var content = await response.Content.ReadAsStringAsync();
        var returnedOrders = JsonSerializer.Deserialize<List<Order>>(content, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        returnedOrders.Should().NotBeNull();
        returnedOrders.Should().HaveCount(2);
        returnedOrders.Should().AllSatisfy(o => o.CustomerEmail.Should().Be(customerEmail));
    }
} 