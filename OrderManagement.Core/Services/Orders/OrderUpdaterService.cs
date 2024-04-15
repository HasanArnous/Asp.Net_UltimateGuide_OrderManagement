﻿using OrderManagement.Core.Domain.RepositoryContracts;
using OrderManagement.Core.ServiceContracts.Orders;
using Microsoft.Extensions.Logging;
using OrderManagement.Core.DTO;

namespace OrderManagement.Core.Services.Orders;

public class OrderUpdaterService : IOrdersUpdaterService
{
    private readonly IOrdersRepository _ordersRepository;
    private readonly ILogger<OrderUpdaterService> _logger;

    public OrderUpdaterService(IOrdersRepository ordersRepository, ILogger<OrderUpdaterService> logger)
    {
        _ordersRepository = ordersRepository;
        _logger = logger;
    }

    public async Task<OrderResponse> UpdateAsync(OrderUpdateRequest orderUpdateRequest)
    {
        var order = orderUpdateRequest.ToOrder();
        var updatedOrder = await _ordersRepository.UpdateAsync(order);
        _logger.LogInformation($"Order Updated, ID: {updatedOrder.OrderId}");
        return updatedOrder.ToOrderResponse();
    }
}