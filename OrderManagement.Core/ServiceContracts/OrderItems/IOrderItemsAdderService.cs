using OrderManagement.Core.DTO;

namespace OrderManagement.Core.ServiceContracts.OrderItems;

public interface IOrderItemsAdderService
{
    Task<OrderItemResponse> AddAsync(OrderItemAddRequest orderItemAddRequest);
}
