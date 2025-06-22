using Microsoft.AspNetCore.Mvc;

namespace BookEShop.CatalogService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BooksController : ControllerBase
    {
        [HttpGet]
        public IActionResult GetAllBooks() => Ok(new[] { new { Id = 1, Title = "Example Book" } });

        [HttpGet("{id}")]
        public IActionResult GetBook(int id) => Ok(new { Id = id, Title = "Example Book" });

        [HttpPost]
        public IActionResult AddBook([FromBody] object book) => Created("/api/books/1", book);

        [HttpPut("{id}")]
        public IActionResult UpdateBook(int id, [FromBody] object book) => NoContent();

        [HttpDelete("{id}")]
        public IActionResult DeleteBook(int id) => NoContent();
    }
} 