using BookStore_Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace BookStore_Domain.Repositories;

public class Category_Repository : ICategory_Repository
{
    private readonly DataContext _context;

    public Category_Repository(DataContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Category>> GetAllCategories()
    {
        return await _context.Categories.ToListAsync();
    }

    public async Task<Category?> GetCategoryById(int id)
    {
        return await _context.Categories.FindAsync(id);
    }

    public async Task<Category> AddCategory(Category category)
    {
        _context.Categories.Add(category);
        await _context.SaveChangesAsync();
        return category;
    }

    public async Task<Category> UpdateCategory(Category category)
    {
        _context.Entry(category).State = EntityState.Modified;
        await _context.SaveChangesAsync();
        return category;
    }

    public async Task DeleteCategory(int id)
    {
        var category = await _context.Categories.FindAsync(id);
        if (category != null)
        {
            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();
        }
    }
} 