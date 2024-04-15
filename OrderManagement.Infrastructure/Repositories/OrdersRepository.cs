﻿using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using OrderManagement.Core.Domain.Entities;
using OrderManagement.Infrastructure.DataStore;
using OrderManagement.Core.Domain.RepositoryContracts;

namespace OrderManagement.Infrastructure.Repositories;

public class OrdersRepository : IOrdersRepository
{
    private readonly AppDbContext _db;
    private readonly ILogger<OrdersRepository> _logger;

    public OrdersRepository(AppDbContext db, ILogger<OrdersRepository> logger)
    {
        _db = db;
        _logger = logger;
    }

    public async Task<Order> CreateAsync(Order order)
    {
        var seq = _db.Orders.Max(o => o.SequentialNumber) + 1;
        order.OrderId = Guid.NewGuid();
        order.OrderDate = DateTime.Now;
        _db.Orders.Add(order);
        await _db.SaveChangesAsync();
        _logger.LogInformation($"New Order is Added ID: {order.OrderId}");
        return order;
    }

    public async Task<bool> DeleteAsync(Guid orderId)
    {
        var order = await _db.Orders.FindAsync(orderId);
        if(order == null)
        {
            _logger.LogWarning($"Trying to remove a not found Order, ID: {orderId}");
            return false;
        }
        _db.Orders.Remove(order);
        await _db.SaveChangesAsync();
        _logger.LogInformation($"Removed an Order, ID: {orderId}");
        return true;
    }

    public async Task<List<Order>> GetAllAsync()
    {
        return await _db.Orders.OrderByDescending(o => o.OrderDate).ToListAsync();
    }

    public async Task<Order> GetAsync(Guid orderId)
    {
        var order = await _db.Orders.FindAsync(orderId);
        if (order == null)
        {
            _logger.LogWarning($"Not found Order, ID: {orderId}");
        }
        return order;    
    }

    public async Task<Order> UpdateAsync(Order order)
    {
        var existOrder = await _db.Orders.FindAsync(order.OrderId);
        if (existOrder == null)
        {
            _logger.LogWarning($"Trying to Update a not found Order, ID: {order.OrderId}");
            return order;
        }
        existOrder.SequentialNumber = order.SequentialNumber;
        existOrder.OrderNumber = order.OrderNumber;
        existOrder.TotalAmount = order.TotalAmount;
        existOrder.CustomerName = order.CustomerName;
        await _db.SaveChangesAsync();
        _logger.LogInformation($"Updated an Order, ID: {order.OrderId}");
        return existOrder;
    }
}