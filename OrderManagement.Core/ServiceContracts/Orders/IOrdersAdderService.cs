using OrderManagement.Core.DTO;

namespace OrderManagement.Core.ServiceContracts.Orders;

public interface IOrdersAdderService
{
    Task<OrderResponse> AddAsync(OrderAddRequest orderAddRequest);
}
