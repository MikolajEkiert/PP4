using Microsoft.AspNetCore.Mvc;
using BookStore_Application.Services;
using BookStore_Domain.Models;

namespace BookStoreService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrdersController : ControllerBase
{
    private readonly IOrder_Service _orderService;

    public OrdersController(IOrder_Service orderService)
    {
        _orderService = orderService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Order>>> GetAllOrders()
    {
        var orders = await _orderService.GetAllOrders();
        return Ok(orders);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Order>> GetOrderById(int id)
    {
        var order = await _orderService.GetOrderById(id);
        if (order == null)
            return NotFound();

        return Ok(order);
    }

    [HttpGet("number/{orderNumber}")]
    public async Task<ActionResult<Order>> GetOrderByOrderNumber(string orderNumber)
    {
        var order = await _orderService.GetOrderByOrderNumber(orderNumber);
        if (order == null)
            return NotFound();

        return Ok(order);
    }

    [HttpGet("customer/{email}")]
    public async Task<ActionResult<IEnumerable<Order>>> GetOrdersByCustomerEmail(string email)
    {
        var orders = await _orderService.GetOrdersByCustomerEmail(email);
        return Ok(orders);
    }

    [HttpPost]
    public async Task<ActionResult<Order>> CreateOrder([FromBody] CreateOrderRequest request)
    {
        try
        {
            var order = await _orderService.CreateOrder(
                request.CustomerEmail,
                request.ShippingAddress,
                request.Items
            );
            return CreatedAtAction(nameof(GetOrderById), new { id = order.Id }, order);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPut("{id}/status")]
    public async Task<IActionResult> UpdateOrderStatus(int id, [FromBody] OrderStatus status)
    {
        try
        {
            await _orderService.UpdateOrderStatus(id, status);
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
        await _orderService.DeleteOrder(id);
        return NoContent();
    }
}

public class CreateOrderRequest
{
    public string CustomerEmail { get; set; } = string.Empty;
    public string ShippingAddress { get; set; } = string.Empty;
    public List<OrderItem> Items { get; set; } = new();
} 