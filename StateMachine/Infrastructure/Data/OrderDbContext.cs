using Microsoft.EntityFrameworkCore;
using StateMachine.Domain;

namespace StateMachine.Infrastructure.Data;

public class OrderDbContext : DbContext
{
    public OrderDbContext(DbContextOptions<OrderDbContext> options) : base(options) { }

    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderStateHistory> OrderStateHistory { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.OrderNumber).IsRequired().HasMaxLength(50);
            entity.Property(e => e.CustomerEmail).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Amount).HasPrecision(18, 2);
            entity.Property(e => e.State).HasConversion<string>();
            entity.HasIndex(e => e.OrderNumber).IsUnique();
        });

        modelBuilder.Entity<OrderStateHistory>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.FromState).HasConversion<string>();
            entity.Property(e => e.ToState).HasConversion<string>();
            entity.HasOne(e => e.Order).WithMany(e => e.StateHistory).HasForeignKey(e => e.OrderId);
        });
    }
}