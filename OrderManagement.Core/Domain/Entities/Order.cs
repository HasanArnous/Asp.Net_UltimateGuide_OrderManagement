﻿using System.ComponentModel.DataAnnotations;

namespace OrderManagement.Core.Domain.Entities;

public class Order
{
    [Key]
    public Guid OrderId { get; set; }
    public string? OrderNumber { get; set; }
    public int SequentialNumber { get; set; }
    [Required]
    [MaxLength(50)]
    public string? CustomerName { get; set; }
    [Required]
    public DateTime OrderDate { get; set; }
    [Range(minimum: 0, maximum: double.MaxValue)]
    public decimal TotalAmount { get; set; }
    public List<OrderItem>? OrderItems { get; set; }

}
