using OrderManagement.Core.Domain.Entities;

namespace OrderManagement.Core.Domain.RepositoryContracts;

public interface IOrderItemsRepository
{
    Task<OrderItem> CreateAsync(OrderItem orderItem);
    Task<OrderItem> UpdateAsync(OrderItem orderItem);
    Task<List<OrderItem>> GetAllAsync();
    Task<OrderItem> GetAsyncById(Guid orderItemId);
    Task<List<OrderItem>> GetAsyncByOrderId(Guid orderId);
    Task<bool> DeleteAsync(Guid orderItemId);
}
