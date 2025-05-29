using Microsoft.AspNetCore.Mvc;
using BookEShop.Application.Interfaces;
using BookEShop.Domain.Models;
using BookEShop.Domain.Enums;
namespace BookEShop.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrdersController(IOrderService orderService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Order>>> GetAllOrders()
    {
        var orders = await orderService.GetAllOrders();
        return Ok(orders);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Order>> GetOrderById(int id)
    {
        var order = await orderService.GetOrderById(id);
        if (order == null)
            return NotFound();

        return Ok(order);
    }

    [HttpGet("number/{orderNumber}")]
    public async Task<ActionResult<Order>> GetOrderByOrderNumber(string orderNumber)
    {
        var order = await orderService.GetOrderByOrderNumber(orderNumber);
        if (order == null)
            return NotFound();

        return Ok(order);
    }

    [HttpGet("customer/{email}")]
    public async Task<ActionResult<IEnumerable<Order>>> GetOrdersByCustomerEmail(string email)
    {
        var orders = await orderService.GetOrdersByCustomerEmail(email);
        return Ok(orders);
    }

    [HttpPost]
    public async Task<ActionResult<Order>> CreateOrder([FromBody] CreateOrderRequest request)
    {
        if (request.Items == null || !request.Items.Any())
            return BadRequest("Order must contain at least one item.");

        var order = await orderService.CreateOrder(
            request.CustomerEmail,
            request.ShippingAddress,
            request.Items
        );

        return CreatedAtAction(nameof(GetOrderById), new { id = order.Id }, order);
    }

    [HttpPut("{id}/status")]
    public async Task<IActionResult> UpdateOrderStatus(int id, [FromBody] OrderStatus status)
    {
        try
        {
            await orderService.UpdateOrderStatus(id, status);
            return NoContent();
        }
        catch (ArgumentException ex)
        {
            return NotFound(ex.Message);
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteOrder(int id)
    {
        await orderService.DeleteOrder(id);
        return NoContent();
    }
}

public class CreateOrderRequest
{
    public string CustomerEmail { get; set; } = string.Empty;
    public string ShippingAddress { get; set; } = string.Empty;
    public List<OrderItem> Items { get; set; } = new();
}