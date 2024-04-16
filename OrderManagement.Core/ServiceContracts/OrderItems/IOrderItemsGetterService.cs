using OrderManagement.Core.DTO;

namespace OrderManagement.Core.ServiceContracts.OrderItems;

public interface IOrderItemsGetterService
{
    Task<OrderItemResponse> GetAsyncByOrderItemId(Guid orderItemId);
    Task<List<OrderItemResponse>> GetAsyncByOrderId(Guid orderId);
    Task<List<OrderItemResponse>> GetAllAsync();
}
