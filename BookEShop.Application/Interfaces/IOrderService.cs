using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BookEShop.Domain.Models;
using BookEShop.Domain.Enums;
namespace BookEShop.Application.Interfaces;

public interface IOrderService
{
    Task<IEnumerable<Order>> GetAllOrders();
    Task<Order?> GetOrderById(int id);
    Task<Order?> GetOrderByOrderNumber(string orderNumber);
    Task<IEnumerable<Order>> GetOrdersByCustomerEmail(string email);
    Task<Order> CreateOrder(string customerEmail, string shippingAddress, List<OrderItem> items);
    Task<Order> UpdateOrderStatus(int orderId, OrderStatus status);
    Task DeleteOrder(int id);
}
