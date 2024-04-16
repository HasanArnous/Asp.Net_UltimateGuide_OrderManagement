using OrderManagement.Core.Domain.Entities;

namespace OrderManagement.Core.DTO;

public class OrderItemResponse
{
    public Guid OrderItemId { get; set; }
    public Guid OrderId { get; set; }
    public string? ProductName { get; set; }
    public int Quantity { get; set; }
    public double UnitPrice { get; set; }
    public double TotalPrice { get; set; }
}

public static class OrderItemExtensions
{
    public static OrderItemResponse ToOrderItemResponse(this OrderItem orderItem)
    {
        return new OrderItemResponse
        {
            OrderId = orderItem.OrderId,
            OrderItemId = orderItem.OrderId,
            ProductName = orderItem.ProductName,
            Quantity = orderItem.Quantity,
            TotalPrice = orderItem.TotalPrice,
            UnitPrice = orderItem.UnitPrice,
        };
    }
}
