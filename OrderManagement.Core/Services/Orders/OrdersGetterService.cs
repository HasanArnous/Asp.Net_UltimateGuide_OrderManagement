using OrderManagement.Core.Domain.RepositoryContracts;
using OrderManagement.Core.ServiceContracts.Orders;
using OrderManagement.Core.DTO;

namespace OrderManagement.Core.Services.Orders;

public class OrdersGetterService : IOrdersGetterService
{
    private readonly IOrdersRepository _ordersRepository;

    public OrdersGetterService(IOrdersRepository ordersRepository)
    {
        _ordersRepository = ordersRepository;
    }

    public async Task<List<OrderResponse>> GetAllAsync()
    {
        return (await _ordersRepository.GetAllAsync()).Select(o => o.ToOrderResponse()).ToList();
    }

    public async Task<OrderResponse?> GetAsync(Guid orderId)
    {
        var order = await _ordersRepository.GetAsync(orderId);
        if (order == null)
            return null;
        return order.ToOrderResponse();
    }
}
