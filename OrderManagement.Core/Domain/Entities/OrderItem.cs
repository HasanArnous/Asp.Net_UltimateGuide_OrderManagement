using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OrderManagement.Core.Domain.Entities
{
    public class OrderItem
    {
        [Key]
        public Guid OrderItemId { get; set; }
        [Required]
        public Guid OrderId { get; set; }
        [ForeignKey(nameof(OrderId))]
        public Order? Order { get; set; }
        [Required]
        [MaxLength(50)]
        public string? ProductName { get; set; }
        [Range(minimum:1, maximum:int.MaxValue)]
        public int Quantity { get; set; }
        [Range(minimum: 0, maximum: double.MaxValue)]
        public decimal UnitPrice { get; set; }
        public decimal TotalPrice { get; set; }
    }
}
