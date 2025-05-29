using BookEShop.Application.Interfaces;
using BookEShop.Domain.Models;
using BookEShop.Domain.Repositories;
using BookEShop.Domain.Enums;


namespace BookEShop.Application.Services;

public class OrderService : IOrderService
{
    private readonly IOrderRepository _orderRepository;
    private readonly IBookRepository _bookRepository;

    public OrderService(IOrderRepository orderRepository, IBookRepository bookRepository)
    {
        _orderRepository = orderRepository;
        _bookRepository = bookRepository;
    }

    public async Task<IEnumerable<Order>> GetAllOrders()
    {
        return await _orderRepository.GetAllOrders();
    }

    public async Task<Order?> GetOrderById(int id)
    {
        return await _orderRepository.GetOrderById(id);
    }

    public async Task<Order?> GetOrderByOrderNumber(string orderNumber)
    {
        return await _orderRepository.GetOrderByOrderNumber(orderNumber);
    }

    public async Task<IEnumerable<Order>> GetOrdersByCustomerEmail(string email)
    {
        return await _orderRepository.GetOrdersByCustomerEmail(email);
    }

    public async Task<Order> CreateOrder(string customerEmail, string shippingAddress, List<OrderItem> items)
    {
        if (items == null || !items.Any())
            throw new ArgumentException("Order must contain at least one item.");

        var bookCache = new Dictionary<int, Book>();

        foreach (var item in items)
        {
            if (!bookCache.TryGetValue(item.BookId, out var book))
            {
                book = await _bookRepository.GetBookById(item.BookId);
                if (book == null)
                    throw new ArgumentException($"Book with ID {item.BookId} does not exist.");
                bookCache[item.BookId] = book;
            }

            if (book.Stock < item.Quantity)
                throw new InvalidOperationException($"Insufficient stock for book ID {item.BookId}.");
        }

        var order = new Order
        {
            CustomerEmail = customerEmail,
            ShippingAddress = shippingAddress,
            OrderItems = items,
            TotalAmount = items.Sum(i => i.Subtotal),
            Status = OrderStatus.Pending,
            OrderNumber = GenerateOrderNumber()
        };

        await _orderRepository.AddOrder(order);

        foreach (var item in items)
        {
            var book = bookCache[item.BookId];
            book.Stock -= item.Quantity;
            await _bookRepository.UpdateBook(book);
        }

        return order;
    }
    public async Task<Order> UpdateOrderStatus(int orderId, OrderStatus status)
    {
        var order = await _orderRepository.GetOrderById(orderId);
        if (order == null)
            throw new ArgumentException($"Order with ID {orderId} not found");

        order.Status = status;
        order.UpdatedAt = DateTime.UtcNow;
        return await _orderRepository.UpdateOrder(order);
    }

    public async Task DeleteOrder(int id)
    {
        await _orderRepository.DeleteOrder(id);
    }

    private string GenerateOrderNumber()
    {
        return $"ORD-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString().Substring(0, 8)}";
    }
}