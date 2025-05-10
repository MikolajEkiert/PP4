using Microsoft.AspNetCore.Mvc;
using BookStore_Application.Services;
using BookStore_Domain.Models;

namespace BookStoreService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BooksController : ControllerBase
{
    private readonly IBook_Service _bookService;

    public BooksController(IBook_Service bookService)
    {
        _bookService = bookService;
    }

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
        var success = await _bookService.UpdateStock(id, quantity);
        if (!success)
            return BadRequest("Invalid stock update");

        return NoContent();
    }
} 