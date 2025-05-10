using BookStore_Domain.Models;

namespace BookStore_Domain.Repositories;

public interface IOrder_Repository
{
    Task<IEnumerable<Order>> GetAllOrders();
    Task<Order?> GetOrderById(int id);
    Task<Order?> GetOrderByOrderNumber(string orderNumber);
    Task<IEnumerable<Order>> GetOrdersByCustomerEmail(string email);
    Task<Order> AddOrder(Order order);
    Task<Order> UpdateOrder(Order order);
    Task DeleteOrder(int id);
} 