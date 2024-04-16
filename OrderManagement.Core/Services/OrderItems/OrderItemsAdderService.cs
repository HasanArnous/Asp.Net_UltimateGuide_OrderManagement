using OrderManagement.Core.ServiceContracts.OrderItems;
using OrderManagement.Core.Domain.RepositoryContracts;
using Microsoft.Extensions.Logging;
using OrderManagement.Core.DTO;

namespace OrderManagement.Core.Services.OrderItems;

public class OrderItemsAdderService : IOrderItemsAdderService
{
    private readonly IOrderItemsRepository _orderItemsRepository;
    private readonly ILogger<OrderItemsAdderService> _logger;

    public OrderItemsAdderService(IOrderItemsRepository orderItemsRepository, ILogger<OrderItemsAdderService> logger)
    {
        _orderItemsRepository = orderItemsRepository;
        _logger = logger;
    }

    public async Task<OrderItemResponse> AddAsync(OrderItemAddRequest orderItemAddRequest)
    {
        var addedItem = await _orderItemsRepository.CreateAsync(orderItemAddRequest.ToOrderItem());
        _logger.LogInformation($"New Order Item was Added, ID: {addedItem.OrderItemId}");
        return addedItem.ToOrderItemResponse();
    }
}
