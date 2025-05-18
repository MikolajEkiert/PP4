using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BookEShop.Domain.Models;
namespace BookEShop.Domain.Repositories;

public interface IOrderRepository
{
    Task<IEnumerable<Order>> GetAllOrders();
    Task<Order?> GetOrderById(int id);
    Task<Order?> GetOrderByOrderNumber(string orderNumber);
    Task<IEnumerable<Order>> GetOrdersByCustomerEmail(string email);
    Task<Order> AddOrder(Order order);
    Task<Order> UpdateOrder(Order order);
    Task DeleteOrder(int id);
}
