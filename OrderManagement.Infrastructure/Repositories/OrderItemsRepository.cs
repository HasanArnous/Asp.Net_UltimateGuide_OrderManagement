using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using OrderManagement.Core.Domain.Entities;
using OrderManagement.Infrastructure.DataStore;
using OrderManagement.Core.Domain.RepositoryContracts;

namespace OrderManagement.Infrastructure.Repositories;

public class OrderItemsRepository : IOrderItemsRepository
{
    private readonly AppDbContext _db;
    private readonly ILogger<OrderItemsRepository> _logger;

    public OrderItemsRepository(AppDbContext db, ILogger<OrderItemsRepository> logger)
    {
        _db = db;
        _logger = logger;
    }

    public async Task<OrderItem> CreateAsync(OrderItem orderItem)
    {
        orderItem.OrderItemId = Guid.NewGuid();
        _db.OrderItems.Add(orderItem);
        await _db.SaveChangesAsync();
        _logger.LogInformation($"New Order Item is Added ID: {orderItem.OrderItemId}");
        return orderItem;
    }

    public async Task<bool> DeleteAsync(Guid orderItemId)
    {
        var orderItem = await _db.OrderItems.FindAsync(orderItemId);
        if(orderItem == null) 
        {
            _logger.LogWarning($"Trying to remove a not found Order Item, ID: {orderItemId}");
            return false;
        }
        _db.OrderItems.Remove(orderItem);
        await _db.SaveChangesAsync();
        _logger.LogInformation($"Removed an Order Item, ID: {orderItemId}");
        return true;
    }

    public async Task<List<OrderItem>> GetAllAsync()
    {
        return await _db.OrderItems.ToListAsync();
    }

    public async Task<OrderItem?> GetAsyncById(Guid orderItemId)
    {
        var orderItem = await _db.OrderItems.FindAsync(orderItemId);
        if (orderItem == null)
            _logger.LogWarning($"Not found Order Item, ID: {orderItemId}");
        return orderItem;
    }

    public async Task<List<OrderItem>> GetAsyncByOrderId(Guid orderId)
    {
        var order = await _db.Orders.Include(o => o.OrderItems).FirstOrDefaultAsync(o => o.OrderId == orderId);
        if(order == null)
        {
            _logger.LogWarning($"Not found Order, ID: {orderId}");
            return new List<OrderItem>();
        }
        if(order.OrderItems == null)
        {
            _logger.LogWarning($"Order without Items!!, ID: {orderId}");
            return new List<OrderItem>();
        }
        return order.OrderItems;
    }

    public async Task<OrderItem> UpdateAsync(OrderItem orderItem)
    {
        var existedItem = await _db.OrderItems.FindAsync(orderItem.OrderItemId);
        if (existedItem == null)
        {
            _logger.LogWarning($"Trying to Update a not found Order Item, ID: {orderItem.OrderItemId}");
            return null;
        }
        existedItem.UnitPrice = orderItem.UnitPrice;
        existedItem.Quantity = orderItem.Quantity;
        existedItem.TotalPrice = orderItem.TotalPrice;
        existedItem.OrderId = orderItem.OrderId;
        await _db.SaveChangesAsync();
        _logger.LogInformation($"Updated an Order Item, ID: {orderItem.OrderItemId}");
        return existedItem;
    }
}
