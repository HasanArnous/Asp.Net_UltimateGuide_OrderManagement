using OrderManagement.Core.Domain.Entities;
using System.ComponentModel.DataAnnotations;

namespace OrderManagement.Core.DTO;

public class OrderUpdateRequest
{
    [Required]
    public Guid OrderId { get; set; }
    public string? OrderNumber { get; set; }
    [Required]
    [MaxLength(50)]
    public string? CustomerName { get; set; }
    [Required]
    public DateTime OrderDate { get; set; }
    public List<OrderItemUpdateRequest> OrderItems { get; set; }
}

public static class OrderUpdateRequestExtensions
{
    public static Order ToOrder(this OrderUpdateRequest orderUpdateRequest)
    {
        return new Order
        {
            OrderId = orderUpdateRequest.OrderId,
            CustomerName = orderUpdateRequest.CustomerName,
            OrderDate = orderUpdateRequest.OrderDate,
            OrderNumber = orderUpdateRequest.OrderNumber,
            TotalAmount = orderUpdateRequest.OrderItems.Sum(oi => oi.Quantity * oi.UnitPrice)
        };
    }
    public static OrderUpdateRequest ToOrderUpdateRequest(this Order order)
    {
        return new OrderUpdateRequest
        {
            CustomerName = order.CustomerName,
            OrderDate = order.OrderDate,
            OrderId = order.OrderId,
            OrderItems = order.OrderItems != null
                ? order.OrderItems.Select(oi => oi.ToOrderItemUpdateRequest()).ToList()
                : new List<OrderItemUpdateRequest>(),
            OrderNumber = order.OrderNumber,
        };
    }
}
