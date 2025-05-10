using Microsoft.AspNetCore.Mvc;
using BookStore_Application.Services;
using BookStore_Domain.Models;

namespace BookStoreService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ShoppingCartController : ControllerBase
{
    private readonly IShoppingCart_Service _shoppingCartService;

    public ShoppingCartController(IShoppingCart_Service shoppingCartService)
    {
        _shoppingCartService = shoppingCartService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ShoppingCart>>> GetAllShoppingCarts()
    {
        var carts = await _shoppingCartService.GetAllShoppingCarts();
        return Ok(carts);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ShoppingCart>> GetShoppingCartById(int id)
    {
        var cart = await _shoppingCartService.GetShoppingCartById(id);
        if (cart == null)
            return NotFound();

        return Ok(cart);
    }

    [HttpGet("customer/{email}")]
    public async Task<ActionResult<ShoppingCart>> GetShoppingCartByCustomerEmail(string email)
    {
        var cart = await _shoppingCartService.GetShoppingCartByCustomerEmail(email);
        if (cart == null)
            return NotFound();

        return Ok(cart);
    }

    [HttpPost]
    public async Task<ActionResult<ShoppingCart>> CreateShoppingCart([FromBody] CreateShoppingCartRequest request)
    {
        try
        {
            var cart = await _shoppingCartService.CreateShoppingCart(request.CustomerEmail);
            return CreatedAtAction(nameof(GetShoppingCartById), new { id = cart.Id }, cart);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<ShoppingCart>> UpdateShoppingCart(int id, [FromBody] ShoppingCart shoppingCart)
    {
        try
        {
            if (id != shoppingCart.Id)
                return BadRequest("ID mismatch");

            var updatedCart = await _shoppingCartService.UpdateShoppingCart(shoppingCart);
            return Ok(updatedCart);
        }
        catch (ArgumentException ex)
        {
            return NotFound(ex.Message);
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteShoppingCart(int id)
    {
        await _shoppingCartService.DeleteShoppingCart(id);
        return NoContent();
    }

    [HttpPost("{cartId}/items")]
    public async Task<ActionResult<CartItem>> AddItemToCart(int cartId, [FromBody] AddCartItemRequest request)
    {
        try
        {
            var cartItem = await _shoppingCartService.AddItemToCart(cartId, request.BookId, request.Quantity);
            return Ok(cartItem);
        }
        catch (ArgumentException ex)
        {
            return NotFound(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPut("items/{itemId}")]
    public async Task<ActionResult<CartItem>> UpdateCartItemQuantity(int itemId, [FromBody] UpdateCartItemRequest request)
    {
        try
        {
            var cartItem = await _shoppingCartService.UpdateCartItemQuantity(itemId, request.Quantity);
            return Ok(cartItem);
        }
        catch (ArgumentException ex)
        {
            return NotFound(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpDelete("items/{itemId}")]
    public async Task<IActionResult> RemoveItemFromCart(int itemId)
    {
        await _shoppingCartService.RemoveItemFromCart(itemId);
        return NoContent();
    }

    [HttpPost("{cartId}/checkout")]
    public async Task<ActionResult<Order>> Checkout(int cartId, [FromBody] CheckoutRequest request)
    {
        try
        {
            var order = await _shoppingCartService.Checkout(cartId, request.ShippingAddress);
            return Ok(order);
        }
        catch (ArgumentException ex)
        {
            return NotFound(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }
}

public class CreateShoppingCartRequest
{
    public string CustomerEmail { get; set; } = string.Empty;
}

public class AddCartItemRequest
{
    public int BookId { get; set; }
    public int Quantity { get; set; }
}

public class UpdateCartItemRequest
{
    public int Quantity { get; set; }
}

public class CheckoutRequest
{
    public string ShippingAddress { get; set; } = string.Empty;
} 