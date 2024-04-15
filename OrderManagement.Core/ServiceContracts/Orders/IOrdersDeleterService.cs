namespace OrderManagement.Core.ServiceContracts.Orders;

public interface IOrdersDeleterService
{
    Task<bool> DeleteAsync(Guid orderId);
}
