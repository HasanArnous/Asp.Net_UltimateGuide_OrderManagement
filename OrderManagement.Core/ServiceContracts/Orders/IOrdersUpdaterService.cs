using OrderManagement.Core.DTO;

namespace OrderManagement.Core.ServiceContracts.Orders;

public interface IOrdersUpdaterService
{
    Task<OrderResponse> UpdateAsync(OrderUpdateRequest orderUpdateRequest);
}
