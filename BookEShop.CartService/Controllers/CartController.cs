using Microsoft.AspNetCore.Mvc;

namespace BookEShop.CartService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CartController : ControllerBase
    {
        [HttpGet("{userId}")]
        public IActionResult GetCart(string userId) => Ok(new { UserId = userId, Items = new object[0] });

        [HttpPost("{userId}/items")]
        public IActionResult AddItem(string userId, [FromBody] object item) => Ok();

        [HttpDelete("{userId}/items/{itemId}")]
        public IActionResult RemoveItem(string userId, int itemId) => NoContent();

        [HttpPost("{userId}/checkout")]
        public IActionResult Checkout(string userId) => Ok(new { OrderId = 1 });
    }
} 