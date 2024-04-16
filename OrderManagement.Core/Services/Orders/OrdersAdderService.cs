using OrderManagement.Core.Domain.RepositoryContracts;
using OrderManagement.Core.ServiceContracts.Orders;
using Microsoft.Extensions.Logging;
using OrderManagement.Core.DTO;

namespace OrderManagement.Core.Services.Orders;

public class OrdersAdderService : IOrdersAdderService
{
    private readonly IOrdersRepository _ordersRepository;
    private readonly IOrderItemsRepository _orderItemsRepository;
    private readonly ILogger<OrdersAdderService> _logger;

    public OrdersAdderService(IOrdersRepository ordersRepository, IOrderItemsRepository orderItemsRepository, ILogger<OrdersAdderService> logger)
    {
        _ordersRepository = ordersRepository;
        _orderItemsRepository = orderItemsRepository;
        _logger = logger;
    }

    public async Task<OrderResponse> AddAsync(OrderAddRequest orderAddRequest)
    {
        var addedOrder = await _ordersRepository.CreateAsync(orderAddRequest.ToOrder());
        var result = addedOrder.ToOrderResponse();

        foreach(var item in orderAddRequest.OrderItems)
        {
            var orderItem = item.ToOrderItem();
            orderItem.OrderId = addedOrder.OrderId;

            var addedOrderItem = await _orderItemsRepository.CreateAsync(orderItem);
            result.OrderItems.Add(addedOrderItem.ToOrderItemResponse());
        }

        _logger.LogInformation($"New Order Added, ID: {result.OrderId}");

        return result;
    }
}
