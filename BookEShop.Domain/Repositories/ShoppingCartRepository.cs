using BookEShop.Domain.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookEShop.Domain.Repositories;

public class ShoppingCartRepository : IShoppingCartRepository
{
    private readonly DataContext _context;

    public ShoppingCartRepository(DataContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<ShoppingCart>> GetAllShoppingCarts()
    {
        return await _context.ShoppingCarts
            .Include(c => c.CartItems)
            .ThenInclude(i => i.Book)
            .ToListAsync();
    }

    public async Task<ShoppingCart?> GetShoppingCartById(int id)
    {
        return await _context.ShoppingCarts
            .Include(c => c.CartItems)
            .ThenInclude(i => i.Book)
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<ShoppingCart?> GetShoppingCartByCustomerEmail(string email)
    {
        return await _context.ShoppingCarts
            .Include(c => c.CartItems)
            .ThenInclude(i => i.Book)
            .FirstOrDefaultAsync(c => c.CustomerEmail == email);
    }

    public async Task<ShoppingCart> AddShoppingCart(ShoppingCart shoppingCart)
    {
        _context.ShoppingCarts.Add(shoppingCart);
        await _context.SaveChangesAsync();
        return shoppingCart;
    }

    public async Task<ShoppingCart> UpdateShoppingCart(ShoppingCart shoppingCart)
    {
        _context.Entry(shoppingCart).State = EntityState.Modified;
        await _context.SaveChangesAsync();
        return shoppingCart;
    }

    public async Task DeleteShoppingCart(int id)
    {
        var shoppingCart = await _context.ShoppingCarts.FindAsync(id);
        if (shoppingCart != null)
        {
            _context.ShoppingCarts.Remove(shoppingCart);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<CartItem?> GetCartItemById(int id)
    {
        return await _context.CartItems
            .Include(i => i.Book)
            .FirstOrDefaultAsync(i => i.Id == id);
    }

    public async Task<CartItem> AddCartItem(CartItem cartItem)
    {
        _context.CartItems.Add(cartItem);
        await _context.SaveChangesAsync();
        return cartItem;
    }

    public async Task<CartItem> UpdateCartItem(CartItem cartItem)
    {
        _context.Entry(cartItem).State = EntityState.Modified;
        await _context.SaveChangesAsync();
        return cartItem;
    }

    public async Task DeleteCartItem(int id)
    {
        var cartItem = await _context.CartItems.FindAsync(id);
        if (cartItem != null)
        {
            _context.CartItems.Remove(cartItem);
            await _context.SaveChangesAsync();
        }
    }
}
