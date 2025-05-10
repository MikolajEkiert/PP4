using BookStore_Domain.Models;

namespace BookStore_Domain.Repositories;

public interface ICategory_Repository
{
    Task<IEnumerable<Category>> GetAllCategories();
    Task<Category?> GetCategoryById(int id);
    Task<Category> AddCategory(Category category);
    Task<Category> UpdateCategory(Category category);
    Task DeleteCategory(int id);
} 