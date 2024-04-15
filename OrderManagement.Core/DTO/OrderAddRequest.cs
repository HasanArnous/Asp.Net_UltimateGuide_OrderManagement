using OrderManagement.Core.Domain.Entities;
using System.ComponentModel.DataAnnotations;

namespace OrderManagement.Core.DTO;

public class OrderAddRequest
{
    [Required]
    [MaxLength(50)]
    public string? CustomerName { get; set; }
    public DateTime OrderDate { get; set; }
    public List<OrderItemAddRequest> OrderItems { get; set; } = new List<OrderItemAddRequest>();
}

public static class OrderAddRequestExtensions
{
    public static Order ToOrder(this OrderAddRequest orderAddRequest)
    {
        return new Order
        {
            CustomerName = orderAddRequest.CustomerName,
            OrderDate = orderAddRequest.OrderDate,
            TotalAmount = orderAddRequest.OrderItems.Sum(oi => oi.Quantity * oi.UnitPrice),
            OrderItems = orderAddRequest.OrderItems.Select(oi => oi.ToOrderItem()).ToList(),
        };
    }
}
