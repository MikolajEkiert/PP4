using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using BookEShop.Application.Interfaces;
using BookEShop.Domain.Models;
using BookEShop.Application.Services;

namespace BookEShop.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BooksController : ControllerBase
{
    private readonly IBookService _bookService;

    public BooksController(IBookService bookService)
    {
        _bookService = bookService;
    }

    [Authorize]
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Book>>> GetAllBooks()
    {
        var books = await _bookService.GetAllBooks();
        return Ok(books);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Book>> GetBookById(int id)
    {
        var book = await _bookService.GetBookById(id);
        if (book == null)
            return NotFound();

        return Ok(book);
    }

    [HttpGet("isbn/{isbn}")]
    public async Task<ActionResult<Book>> GetBookByISBN(string isbn)
    {
        var book = await _bookService.GetBookByISBN(isbn);
        if (book == null)
            return NotFound();

        return Ok(book);
    }

    [HttpPost]
    public async Task<ActionResult<Book>> AddBook(Book book)
    {
        var addedBook = await _bookService.AddBook(book);
        return CreatedAtAction(nameof(GetBookById), new { id = addedBook.Id }, addedBook);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateBook(int id, Book book)
    {
        if (id != book.Id)
            return BadRequest();

        await _bookService.UpdateBook(book);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteBook(int id)
    {
        await _bookService.DeleteBook(id);
        return NoContent();
    }

    [HttpGet("category/{categoryId}")]
    public async Task<ActionResult<IEnumerable<Book>>> GetBooksByCategory(int categoryId)
    {
        var books = await _bookService.GetBooksByCategory(categoryId);
        return Ok(books);
    }

    [HttpGet("search")]
    public async Task<ActionResult<IEnumerable<Book>>> SearchBooks([FromQuery] string term)
    {
        var books = await _bookService.SearchBooks(term);
        return Ok(books);
    }

    [HttpPut("{id}/stock")]
    public async Task<IActionResult> UpdateStock(int id, [FromBody] int quantity)
    {
        if (quantity < 0)
            return BadRequest("Quantity cannot be negative");

        var book = await _bookService.GetBookById(id);
        if (book == null)
            return NotFound();

        if (book.Stock + quantity < 0)
            return BadRequest("Stock cannot be negative");

        var success = await _bookService.UpdateStock(id, quantity);
        if (!success)
            return BadRequest("Invalid stock update");

        return NoContent();
    }
}
