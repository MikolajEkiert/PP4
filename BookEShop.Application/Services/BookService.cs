using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BookEShop.Application.Interfaces;
using BookEShop.Domain.Models;
using BookEShop.Domain.Repositories;

namespace BookEShop.Application.Services;

public class BookService : IBookService
{
    private readonly IBookRepository _bookRepository;

    public BookService(IBookRepository bookRepository)
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
        if (string.IsNullOrWhiteSpace(book.Title))
            throw new ArgumentException("Book title is required.");

        if (book.Price <= 0)
            throw new ArgumentException("Book price must be greater than zero.");

        if (book.Stock < 0)
            throw new ArgumentException("Book stock cannot be negative.");

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

        if (book.Stock + quantity < 0)
            return false;

        book.Stock += quantity;
        await _bookRepository.UpdateBook(book);
        return true;
    }
}