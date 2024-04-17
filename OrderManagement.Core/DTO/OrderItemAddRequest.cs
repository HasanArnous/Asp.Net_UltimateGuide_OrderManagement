using OrderManagement.Core.Domain.Entities;
using System.ComponentModel.DataAnnotations;

namespace OrderManagement.Core.DTO;

public class OrderItemAddRequest
{
    [Required]
    public Guid OrderId { get; set; }
    [Required]
    [MaxLength(50)]
    public string? ProductName { get; set; }
    [Range(minimum: 1, maximum: int.MaxValue)]
    public int Quantity { get; set; }
    [Range(minimum: 0, maximum: double.MaxValue)]
    public double UnitPrice { get; set; }
}

public static class OrderItemAddRequestExtensions
{
    public static OrderItem ToOrderItem(this OrderItemAddRequest orderItemAddRequest)
    {
        return new OrderItem
        {
            OrderId = orderItemAddRequest.OrderId,
            ProductName = orderItemAddRequest.ProductName,
            Quantity = orderItemAddRequest.Quantity,
            UnitPrice = orderItemAddRequest.UnitPrice,
            TotalPrice = orderItemAddRequest.Quantity * orderItemAddRequest.UnitPrice
        };
    }

    public static OrderItemAddRequest ToOrderItemAddRequest(this OrderItem oi)
    {
        return new OrderItemAddRequest
        {
            OrderId = oi.OrderId,
            ProductName = oi.ProductName,
            Quantity = oi.Quantity,
            UnitPrice = oi.UnitPrice,
        };
    }
}
