using OrderManagement.Core.DTO;

namespace OrderManagement.Core.ServiceContracts.OrderItems;

public interface IOrderItemsUpdaterService
{
    Task<OrderItemResponse> UpdateAsync(OrderItemUpdateRequest orderItemUpdateRequest);
}
