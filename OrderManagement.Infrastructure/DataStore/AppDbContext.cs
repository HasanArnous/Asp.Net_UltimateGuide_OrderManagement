using Microsoft.EntityFrameworkCore;
using OrderManagement.Core.Domain.Entities;

namespace OrderManagement.Infrastructure.DataStore;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions options) : base(options) {}
    public AppDbContext() {}

    public virtual DbSet<Order> Orders { get; set; }
    public virtual DbSet<OrderItem> OrderItems { get; set; }
}
