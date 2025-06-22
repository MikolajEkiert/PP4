using Microsoft.AspNetCore.Mvc;

namespace BookEShop.OrderService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrdersController : ControllerBase
    {
        [HttpGet]
        public IActionResult GetAllOrders() => Ok(new[] { new { Id = 1, Status = "Pending" } });

        [HttpGet("{id}")]
        public IActionResult GetOrder(int id) => Ok(new { Id = id, Status = "Pending" });

        [HttpPost]
        public IActionResult CreateOrder([FromBody] object order) => Created("/api/orders/1", order);
    }
} 