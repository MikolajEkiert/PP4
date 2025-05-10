using BookStore_Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace BookStore_Domain.Repositories;

public class Order_Repository : IOrder_Repository
{
    private readonly DataContext _context;

    public Order_Repository(DataContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Order>> GetAllOrders()
    {
        return await _context.Orders
            .Include(o => o.OrderItems)
            .ThenInclude(oi => oi.Book)
            .ToListAsync();
    }

    public async Task<Order?> GetOrderById(int id)
    {
        return await _context.Orders
            .Include(o => o.OrderItems)
            .ThenInclude(oi => oi.Book)
            .FirstOrDefaultAsync(o => o.Id == id);
    }

    public async Task<Order?> GetOrderByOrderNumber(string orderNumber)
    {
        return await _context.Orders
            .Include(o => o.OrderItems)
            .ThenInclude(oi => oi.Book)
            .FirstOrDefaultAsync(o => o.OrderNumber == orderNumber);
    }

    public async Task<IEnumerable<Order>> GetOrdersByCustomerEmail(string email)
    {
        return await _context.Orders
            .Include(o => o.OrderItems)
            .ThenInclude(oi => oi.Book)
            .Where(o => o.CustomerEmail == email)
            .ToListAsync();
    }

    public async Task<Order> AddOrder(Order order)
    {
        _context.Orders.Add(order);
        await _context.SaveChangesAsync();
        return order;
    }

    public async Task<Order> UpdateOrder(Order order)
    {
        _context.Entry(order).State = EntityState.Modified;
        await _context.SaveChangesAsync();
        return order;
    }

    public async Task DeleteOrder(int id)
    {
        var order = await _context.Orders.FindAsync(id);
        if (order != null)
        {
            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();
        }
    }
} 