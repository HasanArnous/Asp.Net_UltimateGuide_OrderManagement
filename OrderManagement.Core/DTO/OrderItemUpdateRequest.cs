using OrderManagement.Core.Domain.Entities;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace OrderManagement.Core.DTO;

public class OrderItemUpdateRequest
{
    [Required]
    public Guid OrderItemId { get; set; }
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

public static class OrderItemUpdateRequestExtensions
{
    public static OrderItem ToOrderItem(this OrderItemUpdateRequest orderItemUpdateRequest)
    {
        return new OrderItem
        {
            OrderId = orderItemUpdateRequest.OrderId,
            OrderItemId = orderItemUpdateRequest.OrderItemId,
            ProductName = orderItemUpdateRequest.ProductName,
            Quantity = orderItemUpdateRequest.Quantity,
            UnitPrice = orderItemUpdateRequest.UnitPrice,
            TotalPrice = orderItemUpdateRequest.Quantity * orderItemUpdateRequest.UnitPrice
        };
    }
    public static OrderItemUpdateRequest ToOrderItemUpdateRequest(this OrderItem orderItem)
    {
        return new OrderItemUpdateRequest
        {
            OrderId = orderItem.OrderId,
            OrderItemId = orderItem.OrderItemId,
            ProductName = orderItem.ProductName,
            Quantity = orderItem.Quantity,
            UnitPrice = orderItem.UnitPrice,
        };
    }
}
