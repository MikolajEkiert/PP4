using BookStore_Domain.Models;

namespace BookStore_Domain.Repositories;

public interface IBook_Repository
{
    Task<IEnumerable<Book>> GetAllBooks();
    Task<Book?> GetBookById(int id);
    Task<Book?> GetBookByISBN(string isbn);
    Task<Book> AddBook(Book book);
    Task<Book> UpdateBook(Book book);
    Task DeleteBook(int id);
    Task<IEnumerable<Book>> GetBooksByCategory(int categoryId);
    Task<IEnumerable<Book>> SearchBooks(string searchTerm);
} 