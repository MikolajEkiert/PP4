using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BookEShop.Domain.Models;

namespace BookEShop.Domain.Repositories;

public interface ICategoryRepository
{
    #region Category
    Task<List<Category>> GetAllCategoriesAsync();
    Task<Category?> GetCategoryById(int id);
    Task<Category> AddCategory(Category category);
    Task<Category> UpdateCategory(Category category);
    Task DeleteCategory(int id);
    #endregion
}
