using OrderManagement.Core.DTO;

namespace OrderManagement.Core.ServiceContracts.Order;

public interface IOrdersAdderService
{
    Task<OrderResponse> AddAsync(OrderAddRequest orderAddRequest);
}
