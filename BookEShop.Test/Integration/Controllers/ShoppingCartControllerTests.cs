using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using BookEShop.Domain.Models;
using BookEShop.Test.Integration;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

namespace BookEShop.Test.Integration.Controllers;

public class ShoppingCartControllerTests : TestBase
{
    private readonly ITestOutputHelper _testOutputHelper;

    public ShoppingCartControllerTests(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
    }

    [Fact]
    public async Task GetAllShoppingCarts_ShouldReturnAllCarts()
    {
        // Act
        var response = await Client.GetAsync("/api/ShoppingCart");
        var content = await response.Content.ReadAsStringAsync();
        var carts = JsonSerializer.Deserialize<List<ShoppingCart>>(content, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        carts.Should().NotBeNull();
    }

    [Fact]
    public async Task GetShoppingCartById_WhenCartExists_ShouldReturnCart()
    {
        // Arrange
        var cart = new ShoppingCart
        {
            CustomerEmail = "test@example.com",
            TotalAmount = 0
        };
        Context.ShoppingCarts.Add(cart);
        await Context.SaveChangesAsync();

        // Act
        var response = await Client.GetAsync($"/api/ShoppingCart/{cart.Id}");
        var content = await response.Content.ReadAsStringAsync();
        var returnedCart = JsonSerializer.Deserialize<ShoppingCart>(content, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        returnedCart.Should().NotBeNull();
        returnedCart!.Id.Should().Be(cart.Id);
        returnedCart.CustomerEmail.Should().Be(cart.CustomerEmail);
    }

    [Fact]
    public async Task GetShoppingCartById_WhenCartDoesNotExist_ShouldReturnNotFound()
    {
        // Act
        var response = await Client.GetAsync("/api/ShoppingCart/999");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task CreateShoppingCart_WithValidData_ShouldCreateCart()
    {
        // Arrange
        var request = new
        {
            CustomerEmail = "test@example.com"
        };

        var content = new StringContent(
            JsonSerializer.Serialize(request),
            Encoding.UTF8,
            "application/json");

        // Act
        var response = await Client.PostAsync("/api/ShoppingCart", content);
        var responseContent = await response.Content.ReadAsStringAsync();
        var createdCart = JsonSerializer.Deserialize<ShoppingCart>(responseContent, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        createdCart.Should().NotBeNull();
        createdCart!.CustomerEmail.Should().Be(request.CustomerEmail);
        createdCart.TotalAmount.Should().Be(0);
        createdCart.CartItems.Should().BeEmpty();
    }

    [Fact]
    public async Task CreateShoppingCart_WithExistingCustomerEmail_ShouldReturnBadRequest()
    {
        // Arrange
        var existingCart = new ShoppingCart
        {
            CustomerEmail = "test@example.com",
            TotalAmount = 0
        };
        Context.ShoppingCarts.Add(existingCart);
        await Context.SaveChangesAsync();

        var request = new
        {
            CustomerEmail = "test@example.com"
        };

        var content = new StringContent(
            JsonSerializer.Serialize(request),
            Encoding.UTF8,
            "application/json");

        // Act
        var response = await Client.PostAsync("/api/ShoppingCart", content);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }


    [Fact]
    public async Task AddItemToCart_WithInsufficientStock_ShouldReturnBadRequest()
    {
        // Arrange
        var cart = new ShoppingCart
        {
            CustomerEmail = "test@example.com",
            TotalAmount = 0
        };
        Context.ShoppingCarts.Add(cart);

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
            BookId = book.Id,
            Quantity = 10
        };

        var content = new StringContent(
            JsonSerializer.Serialize(request),
            Encoding.UTF8,
            "application/json");

        // Act
        var response = await Client.PostAsync($"/api/ShoppingCart/{cart.Id}/items", content);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }


    [Fact]
    public async Task Checkout_WithEmptyCart_ShouldReturnBadRequest()
    {
        // Arrange
        var cart = new ShoppingCart
        {
            CustomerEmail = "test@example.com",
            TotalAmount = 0
        };
        Context.ShoppingCarts.Add(cart);
        await Context.SaveChangesAsync();

        var request = new
        {
            ShippingAddress = "123 Test St"
        };

        var content = new StringContent(
            JsonSerializer.Serialize(request),
            Encoding.UTF8,
            "application/json");

        // Act
        var response = await Client.PostAsync($"/api/ShoppingCart/{cart.Id}/checkout", content);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
} 