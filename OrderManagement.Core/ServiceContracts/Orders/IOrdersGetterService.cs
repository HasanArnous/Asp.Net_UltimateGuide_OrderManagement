using OrderManagement.Core.DTO;

namespace OrderManagement.Core.ServiceContracts.Orders;

public interface IOrdersGetterService
{
    Task<OrderResponse> GetAsync(Guid orderId);
    Task<List<OrderResponse>> GetAllAsync();
}
