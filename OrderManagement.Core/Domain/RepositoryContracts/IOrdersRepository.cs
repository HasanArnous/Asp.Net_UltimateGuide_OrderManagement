using OrderManagement.Core.Domain.Entities;

namespace OrderManagement.Core.Domain.RepositoryContracts;

public interface IOrdersRepository
{
    Task<Order> CreateAsync(Order order);
    Task<Order> UpdateAsync(Order order);
    Task<List<Order>> GetAllAsync();
    Task<Order> GetAsync(Guid orderId);
    Task<bool> DeleteAsync(Guid orderId);
}
