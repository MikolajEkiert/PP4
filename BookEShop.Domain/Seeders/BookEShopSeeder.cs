using BookEShop.Domain.Models;
using BookEShop.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookEShop.Domain.Seeders;

public class BookEShopSeeder(DataContext _context) : IBookEShopSeeder
{

    public async Task Seed()
    {
        if (!_context.Categories.Any())
        {
            var categories = new List<Category>
            {
                new Category { Name = "Fiction", Description = "Fiction books" },
                new Category { Name = "Non-Fiction", Description = "Non-fiction books" },
                new Category { Name = "Science", Description = "Science books" },
                new Category { Name = "History", Description = "History books" }
            };

            await _context.Categories.AddRangeAsync(categories);
            await _context.SaveChangesAsync();
        }

        if (!_context.Books.Any())
        {
            var fictionCategory = await _context.Categories.FirstOrDefaultAsync(c => c.Name == "Fiction");
            var scienceCategory = await _context.Categories.FirstOrDefaultAsync(c => c.Name == "Science");

            if (fictionCategory == null || scienceCategory == null)
            {
                throw new InvalidOperationException("Required categories not found in the database.");
            }

            var books = new List<Book>
            {
                new Book
                {
                    Title = "The Great Gatsby",
                    Author = "F. Scott Fitzgerald",
                    ISBN = "978-0743273565",
                    Price = 9.99m,
                    Stock = 50,
                    Sku = "TG001",
                    BookCategory = fictionCategory,
                    PublicationYear = 2004
                },
                new Book
                {
                    Title = "A Brief History of Time",
                    Author = "Stephen Hawking",
                    ISBN = "978-0553380163",
                    Price = 14.99m,
                    Stock = 30,
                    Sku = "BH001",
                    BookCategory = scienceCategory,
                    PublicationYear = 1998
                }
            };

            await _context.Books.AddRangeAsync(books);
            await _context.SaveChangesAsync();
        }
    }
}