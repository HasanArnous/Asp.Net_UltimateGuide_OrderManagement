using OrderManagement.Core.ServiceContracts.OrderItems;
using OrderManagement.Core.Domain.RepositoryContracts;
using OrderManagement.Core.DTO;

namespace OrderManagement.Core.Services.OrderItems;

public class OrderItemsGetterService : IOrderItemsGetterService
{
    private readonly IOrderItemsRepository _orderItemsRepository;

    public OrderItemsGetterService(IOrderItemsRepository orderItemsRepository)
    {
        _orderItemsRepository = orderItemsRepository;
    }

    public async Task<List<OrderItemResponse>> GetAllAsync()
    {
        return (await  _orderItemsRepository.GetAllAsync())
            .Select(oi => oi.ToOrderItemResponse())
            .ToList();
    }

    public async Task<List<OrderItemResponse>> GetAsyncByOrderId(Guid orderId)
    {
        return (await _orderItemsRepository.GetAsyncByOrderId(orderId))
            .Select(oi => oi.ToOrderItemResponse())
            .ToList();
    }

    public async Task<OrderItemResponse> GetAsyncByOrderItemId(Guid orderItemId)
    {
        return (await _orderItemsRepository.GetAsyncById(orderItemId)).ToOrderItemResponse();
    }
}
