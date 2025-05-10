using BookStore_Domain.Models;
using BookStore_Domain.Repositories;

namespace BookStore_Application.Services;

public class Book_Service : IBook_Service
{
    private readonly IBook_Repository _bookRepository;

    public Book_Service(IBook_Repository bookRepository)
    {
        _bookRepository = bookRepository;
    }

    public async Task<IEnumerable<Book>> GetAllBooks()
    {
        return await _bookRepository.GetAllBooks();
    }

    public async Task<Book?> GetBookById(int id)
    {
        return await _bookRepository.GetBookById(id);
    }

    public async Task<Book?> GetBookByISBN(string isbn)
    {
        return await _bookRepository.GetBookByISBN(isbn);
    }

    public async Task<Book> AddBook(Book book)
    {
        return await _bookRepository.AddBook(book);
    }

    public async Task<Book> UpdateBook(Book book)
    {
        return await _bookRepository.UpdateBook(book);
    }

    public async Task DeleteBook(int id)
    {
        await _bookRepository.DeleteBook(id);
    }

    public async Task<IEnumerable<Book>> GetBooksByCategory(int categoryId)
    {
        return await _bookRepository.GetBooksByCategory(categoryId);
    }

    public async Task<IEnumerable<Book>> SearchBooks(string searchTerm)
    {
        return await _bookRepository.SearchBooks(searchTerm);
    }

    public async Task<bool> UpdateStock(int bookId, int quantity)
    {
        var book = await _bookRepository.GetBookById(bookId);
        if (book == null)
            return false;

        book.Stock += quantity;
        if (book.Stock < 0)
            return false;

        await _bookRepository.UpdateBook(book);
        return true;
    }
} 