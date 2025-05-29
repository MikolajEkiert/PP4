using Xunit;
using Moq;
using FluentAssertions;
using BookEShop.Application.Services;
using BookEShop.Application.Interfaces;
using BookEShop.Domain.Models;
using BookEShop.Domain.Repositories;
using Xunit.Abstractions;

namespace BookEShop.Test.Unit.Services;

public class BookServiceTests
{
    private readonly ITestOutputHelper _testOutputHelper;
    private readonly Mock<IBookRepository> _bookRepositoryMock;
    private readonly IBookService _bookService;

    public BookServiceTests(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
        _bookRepositoryMock = new Mock<IBookRepository>();
        _bookService = new BookService(_bookRepositoryMock.Object);
    }

    [Fact]
    public async Task GetAllBooks_ShouldReturnAllBooks()
    {
        // Arrange
        var expectedBooks = new List<Book>
        {
            new() { Id = 1, Title = "Book 1", ISBN = "1234567890", Price = 29.99m, Stock = 10 },
            new() { Id = 2, Title = "Book 2", ISBN = "0987654321", Price = 39.99m, Stock = 5 }
        };
        _bookRepositoryMock.Setup(repo => repo.GetAllBooks())
            .ReturnsAsync(expectedBooks);

        // Act
        var result = await _bookService.GetAllBooks();

        // Assert
        result.Should().BeEquivalentTo(expectedBooks);
        _bookRepositoryMock.Verify(repo => repo.GetAllBooks(), Times.Once);
    }

    [Fact]
    public async Task GetBookById_WhenBookExists_ShouldReturnBook()
    {
        // Arrange
        var expectedBook = new Book { Id = 1, Title = "Test Book", ISBN = "1234567890", Price = 29.99m, Stock = 10 };
        _bookRepositoryMock.Setup(repo => repo.GetBookById(1))
            .ReturnsAsync(expectedBook);

        // Act
        var result = await _bookService.GetBookById(1);

        // Assert
        result.Should().BeEquivalentTo(expectedBook);
        _bookRepositoryMock.Verify(repo => repo.GetBookById(1), Times.Once);
    }

    [Fact]
    public async Task GetBookById_WhenBookDoesNotExist_ShouldReturnNull()
    {
        // Arrange
        _bookRepositoryMock.Setup(repo => repo.GetBookById(999))
            .ReturnsAsync((Book?)null);

        // Act
        var result = await _bookService.GetBookById(999);

        // Assert
        result.Should().BeNull();
        _bookRepositoryMock.Verify(repo => repo.GetBookById(999), Times.Once);
    }

    [Fact]
    public async Task GetBookByISBN_WhenBookExists_ShouldReturnBook()
    {
        // Arrange
        var expectedBook = new Book { Id = 1, Title = "Test Book", ISBN = "1234567890", Price = 29.99m, Stock = 10 };
        _bookRepositoryMock.Setup(repo => repo.GetBookByISBN("1234567890"))
            .ReturnsAsync(expectedBook);

        // Act
        var result = await _bookService.GetBookByISBN("1234567890");

        // Assert
        result.Should().BeEquivalentTo(expectedBook);
        _bookRepositoryMock.Verify(repo => repo.GetBookByISBN("1234567890"), Times.Once);
    }

    [Fact]
    public async Task AddBook_ShouldReturnAddedBook()
    {
        // Arrange
        var bookToAdd = new Book { Title = "New Book", ISBN = "1234567890", Price = 29.99m, Stock = 10 };
        var expectedBook = new Book { Id = 1, Title = "New Book", ISBN = "1234567890", Price = 29.99m, Stock = 10 };
        _bookRepositoryMock.Setup(repo => repo.AddBook(bookToAdd))
            .ReturnsAsync(expectedBook);

        // Act
        var result = await _bookService.AddBook(bookToAdd);

        // Assert
        result.Should().BeEquivalentTo(expectedBook);
        _bookRepositoryMock.Verify(repo => repo.AddBook(bookToAdd), Times.Once);
    }

    [Fact]
    public async Task UpdateBook_ShouldReturnUpdatedBook()
    {
        // Arrange
        var bookToUpdate = new Book { Id = 1, Title = "Updated Book", ISBN = "1234567890", Price = 39.99m, Stock = 5 };
        _bookRepositoryMock.Setup(repo => repo.UpdateBook(bookToUpdate))
            .ReturnsAsync(bookToUpdate);

        // Act
        var result = await _bookService.UpdateBook(bookToUpdate);

        // Assert
        result.Should().BeEquivalentTo(bookToUpdate);
        _bookRepositoryMock.Verify(repo => repo.UpdateBook(bookToUpdate), Times.Once);
    }

    [Fact]
    public async Task UpdateStock_WhenBookExistsAndStockIsSufficient_ShouldReturnTrue()
    {
        // Arrange
        var book = new Book { Id = 1, Title = "Test Book", ISBN = "1234567890", Price = 29.99m, Stock = 10 };
        _bookRepositoryMock.Setup(repo => repo.GetBookById(1))
            .ReturnsAsync(book);
        _bookRepositoryMock.Setup(repo => repo.UpdateBook(It.IsAny<Book>()))
            .ReturnsAsync((Book b) => b);

        // Act
        var result = await _bookService.UpdateStock(1, -5);

        // Assert
        result.Should().BeTrue();
        _testOutputHelper.WriteLine(book.Stock.ToString(), book);
        book.Stock.Should().Be(5);
        _bookRepositoryMock.Verify(repo => repo.GetBookById(1), Times.Once);
        _bookRepositoryMock.Verify(repo => repo.UpdateBook(It.IsAny<Book>()), Times.Once);
    }

    [Fact]
    public async Task UpdateStock_WhenBookDoesNotExist_ShouldReturnFalse()
    {
        // Arrange
        _bookRepositoryMock.Setup(repo => repo.GetBookById(999))
            .ReturnsAsync((Book?)null);

        // Act
        var result = await _bookService.UpdateStock(999, 5);

        // Assert
        result.Should().BeFalse();
        _bookRepositoryMock.Verify(repo => repo.GetBookById(999), Times.Once);
        _bookRepositoryMock.Verify(repo => repo.UpdateBook(It.IsAny<Book>()), Times.Never);
    }

    [Fact]
    public async Task UpdateStock_WhenStockWouldBecomeNegative_ShouldReturnFalse()
    {
        // Arrange
        var book = new Book { Id = 1, Title = "Test Book", ISBN = "1234567890", Price = 29.99m, Stock = 5 };
        _bookRepositoryMock.Setup(repo => repo.GetBookById(1))
            .ReturnsAsync(book);

        // Act
        var result = await _bookService.UpdateStock(1, -10);
        // Assert
        result.Should().BeFalse();
        _testOutputHelper.WriteLine(book.Stock.ToString(),book);
        book.Stock.Should().Be(5); // Stock should remain unchanged
        _bookRepositoryMock.Verify(repo => repo.GetBookById(1), Times.Once);
        _bookRepositoryMock.Verify(repo => repo.UpdateBook(It.IsAny<Book>()), Times.Never);
    }
} 