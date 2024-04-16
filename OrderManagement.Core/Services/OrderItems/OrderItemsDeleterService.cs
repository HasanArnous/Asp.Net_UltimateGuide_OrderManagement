using Microsoft.Extensions.Logging;
using OrderManagement.Core.Domain.Entities;
using OrderManagement.Core.Domain.RepositoryContracts;
using OrderManagement.Core.ServiceContracts.OrderItems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderManagement.Core.Services.OrderItems;

public class OrderItemsDeleterService : IOrderItemsDeleterService
{
    private readonly IOrderItemsRepository _orderItemsRepository;
    private readonly ILogger<OrderItemsDeleterService> _logger;

    public OrderItemsDeleterService(IOrderItemsRepository orderItemsRepository, ILogger<OrderItemsDeleterService> logger)
    {
        _orderItemsRepository = orderItemsRepository;
        _logger = logger;
    }

    public async Task<bool> DeleteAsync(Guid orderItemId)
    {
        var result = await _orderItemsRepository.DeleteAsync(orderItemId);
        if (result)
            _logger.LogInformation($"Order Item Deleted with ID: {orderItemId}");
        else
            _logger.LogInformation($"Couldn't Delete Order Item with ID: {orderItemId}");
        return result;
    }
}
