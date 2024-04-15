using OrderManagement.Core.Domain.Entities;

namespace OrderManagement.Core.DTO;

public class OrderResponse
{
    public Guid OrderId { get; set; }
    public string? OrderNumber { get; set; }
    public string? CustomerName { get; set; }
    public DateTime OrderDate { get; set; }
    public decimal TotalAmount { get; set; }
    public List<OrderItemResponse> OrderItems { get; set; }
}

public static class OrderExtensions
{
    public static OrderResponse ToOrderResponse(this Order order)
    {
        return new OrderResponse
        {
            OrderId = order.OrderId,
            OrderNumber = order.OrderNumber,
            CustomerName = order.CustomerName,
            OrderDate = order.OrderDate,
            TotalAmount = order.TotalAmount,
            OrderItems = order.OrderItems.Select(oi => oi.)
        };
    }
}
