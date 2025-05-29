using System.Net;
using System.Text;
using System.Text.Json;
using BookEShop.Domain.Models;
using BookEShop.Test.Integration;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace BookEShop.Test.Integration.Controllers;

public class BooksControllerTests : TestBase
{
    [Fact]
    public async Task GetAllBooks_ShouldReturnAllBooks()
    {
        // Act
        var response = await Client.GetAsync("/api/Books");
        var content = await response.Content.ReadAsStringAsync();
        var books = JsonSerializer.Deserialize<List<Book>>(content, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        books.Should().NotBeNull();
        books.Should().NotBeEmpty();
    }
    [Fact]
    public async Task GetBookById_WhenBookDoesNotExist_ShouldReturnNotFound()
    {
        // Act
        var response = await Client.GetAsync("/api/Books/999");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task UpdateBook_WithValidData_ShouldUpdateBook()
    {
        // Arrange
        var book = new Book
        {
            Title = "Original Book",
            ISBN = "1234567890",
            Price = 29.99m,
            Stock = 10
        };
        Context.Books.Add(book);
        await Context.SaveChangesAsync();

        book.Title = "Updated Book";
        book.Price = 39.99m;
        var content = new StringContent(
            JsonSerializer.Serialize(book),
            Encoding.UTF8,
            "application/json");

        // Act
        var response = await Client.PutAsync($"/api/Books/{book.Id}", content);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var updatedBook = await Context.Books.FindAsync(book.Id);
        updatedBook.Should().NotBeNull();
        updatedBook!.Title.Should().Be("Updated Book");
        updatedBook.Price.Should().Be(39.99m);
    }
 


} 