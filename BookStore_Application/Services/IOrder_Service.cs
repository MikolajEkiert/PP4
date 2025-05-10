using BookStore_Domain.Models;

namespace BookStore_Application.Services;

public interface IOrder_Service
{
    Task<IEnumerable<Order>> GetAllOrders();
    Task<Order?> GetOrderById(int id);
    Task<Order?> GetOrderByOrderNumber(string orderNumber);
    Task<IEnumerable<Order>> GetOrdersByCustomerEmail(string email);
    Task<Order> CreateOrder(string customerEmail, string shippingAddress, List<OrderItem> items);
    Task<Order> UpdateOrderStatus(int orderId, OrderStatus status);
    Task DeleteOrder(int id);
} 