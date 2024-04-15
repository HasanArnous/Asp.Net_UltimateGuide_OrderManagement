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
    public decimal UnitPrice { get; set; }
}
