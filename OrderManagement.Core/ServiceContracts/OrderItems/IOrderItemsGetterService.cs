using OrderManagement.Core.DTO;

namespace OrderManagement.Core.ServiceContracts.OrderItems;

public interface IOrderItemsGetterService
{
    Task<OrderResponse> GetAsyncByOrderItemId(Guid orderItemId);
    Task<List<OrderResponse>> GetAsyncByOrderId(Guid orderId);
    Task<List<OrderResponse>> GetAllAsync();
}
