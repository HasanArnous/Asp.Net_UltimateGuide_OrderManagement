namespace OrderManagement.Core.ServiceContracts.OrderItems;

public interface IOrderItemsDeleterService
{
    Task<bool> DeleteAsync(Guid orderItemId);
}
