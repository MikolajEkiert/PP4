using BookEShop.Domain.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookEShop.Domain.Repositories;

public class BookRepository(DataContext _context) : IBookRepository
{

    public async Task<List<Book>> GetAllBooks()
    {
        return await _context.Books.Include(b => b.BookCategory).ToListAsync();
    }

    public async Task<Book?> GetBookById(int id)
    {
        return await _context.Books.Include(b => b.BookCategory).FirstOrDefaultAsync(b => b.Id == id);
    }

    public async Task<Book?> GetBookByISBN(string isbn)
    {
        return await _context.Books.Include(b => b.BookCategory).FirstOrDefaultAsync(b => b.ISBN == isbn);
    }

    public async Task<Book> AddBook(Book book)
    {
        _context.Books.Add(book);
        await _context.SaveChangesAsync();
        return book;
    }

    public async Task<Book> UpdateBook(Book book)
    {
        _context.Entry(book).State = EntityState.Modified;
        await _context.SaveChangesAsync();
        return book;
    }

    public async Task DeleteBook(int id)
    {
        var book = await _context.Books.FindAsync(id);
        if (book != null)
        {
            _context.Books.Remove(book);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<IEnumerable<Book>> GetBooksByCategory(int categoryId)
    {
        return await _context.Books.Include(b => b.BookCategory)
            .Where(b => b.BookCategory.Id == categoryId)
            .ToListAsync();
    }

    public async Task<IEnumerable<Book>> SearchBooks(string searchTerm)
    {
        return await _context.Books.Include(b => b.BookCategory)
            .Where(b => b.Title.Contains(searchTerm) ||
                       b.Author.Contains(searchTerm) ||
                       b.ISBN.Contains(searchTerm))
            .ToListAsync();
    }
}