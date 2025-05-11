using BookEShop.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookEShop.Application.Interfaces;

public interface IBookService
{
    Task<IEnumerable<Book>> GetAllBooks();
    Task<Book?> GetBookById(int id);
    Task<Book?> GetBookByISBN(string isbn);
    Task<Book> AddBook(Book book);
    Task<Book> UpdateBook(Book book);
    Task DeleteBook(int id);
    Task<IEnumerable<Book>> GetBooksByCategory(int categoryId);
    Task<IEnumerable<Book>> SearchBooks(string searchTerm);
    Task<bool> UpdateStock(int bookId, int quantity);
}
