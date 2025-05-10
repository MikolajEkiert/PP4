using Microsoft.AspNetCore.Mvc;
using BookStore_Domain.Models;
using BookStore_Domain.Repositories;

namespace BookStoreService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CategoriesController : ControllerBase
{
    private readonly ICategory_Repository _categoryRepository;

    public CategoriesController(ICategory_Repository categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Category>>> GetAllCategories()
    {
        var categories = await _categoryRepository.GetAllCategories();
        return Ok(categories);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Category>> GetCategoryById(int id)
    {
        var category = await _categoryRepository.GetCategoryById(id);
        if (category == null)
            return NotFound();

        return Ok(category);
    }

    [HttpPost]
    public async Task<ActionResult<Category>> AddCategory(Category category)
    {
        var addedCategory = await _categoryRepository.AddCategory(category);
        return CreatedAtAction(nameof(GetCategoryById), new { id = addedCategory.Id }, addedCategory);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateCategory(int id, Category category)
    {
        if (id != category.Id)
            return BadRequest();

        await _categoryRepository.UpdateCategory(category);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteCategory(int id)
    {
        await _categoryRepository.DeleteCategory(id);
        return NoContent();
    }
} 