using OrderManagement.Core.ServiceContracts.OrderItems;
using OrderManagement.Core.Domain.RepositoryContracts;
using Microsoft.Extensions.Logging;
using OrderManagement.Core.DTO;

namespace OrderManagement.Core.Services.OrderItems;

public class OrderItemsUpdaterService : IOrderItemsUpdaterService
{
    private readonly IOrderItemsRepository _orderItemsRepository;
    private readonly ILogger<OrderItemsUpdaterService> _logger;

    public OrderItemsUpdaterService(IOrderItemsRepository orderItemsRepository, ILogger<OrderItemsUpdaterService> logger)
    {
        _orderItemsRepository = orderItemsRepository;
        _logger = logger;
    }

    public async Task<OrderItemResponse> UpdateAsync(OrderItemUpdateRequest orderItemUpdateRequest)
    {
        var updatedItem = await _orderItemsRepository.UpdateAsync(orderItemUpdateRequest.ToOrderItem());
        _logger.LogInformation($"Order Item Updated, ID: {updatedItem.OrderItemId}");
        return updatedItem.ToOrderItemResponse();
    }
}
