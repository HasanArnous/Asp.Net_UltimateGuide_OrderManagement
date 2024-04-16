using OrderManagement.Core.Domain.RepositoryContracts;
using OrderManagement.Core.ServiceContracts.Orders;
using Microsoft.Extensions.Logging;

namespace OrderManagement.Core.Services.Orders;

public class OrdersDeleterService : IOrdersDeleterService
{
    private readonly IOrdersRepository _ordersRepository;
    private readonly ILogger<OrdersDeleterService> _logger;

    public OrdersDeleterService(IOrdersRepository ordersRepository, ILogger<OrdersDeleterService> logger)
    {
        _ordersRepository = ordersRepository;
        _logger = logger;
    }

    public async Task<bool> DeleteAsync(Guid orderId)
    {
        var result = await _ordersRepository.DeleteAsync(orderId);
        if (result)
            _logger.LogInformation($"Order Deleted with ID: {orderId}");
        else
            _logger.LogInformation($"Couldn't Delete Order with ID: {orderId}");
        return result;
    }
}
