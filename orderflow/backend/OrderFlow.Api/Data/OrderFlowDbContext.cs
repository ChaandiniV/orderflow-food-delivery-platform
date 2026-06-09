using Microsoft.EntityFrameworkCore;
using OrderFlow.Api.Domain.Entities;

namespace OrderFlow.Api.Data;

public class OrderFlowDbContext : DbContext
{
    public OrderFlowDbContext(DbContextOptions<OrderFlowDbContext> options) : base(options)
    {
    }

    public DbSet<Restaurant> Restaurants => Set<Restaurant>();
    public DbSet<MenuItem> MenuItems => Set<MenuItem>();
    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderItem> OrderItems => Set<OrderItem>();
    public DbSet<OrderStatusEvent> OrderStatusEvents => Set<OrderStatusEvent>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Restaurant>(entity =>
        {
            entity.HasKey(r => r.Id);
            entity.Property(r => r.Name).HasMaxLength(120).IsRequired();
            entity.Property(r => r.Cuisine).HasMaxLength(80).IsRequired();
            entity.Property(r => r.Area).HasMaxLength(120).IsRequired();
            entity.Property(r => r.Rating).HasPrecision(3, 2);
        });

        modelBuilder.Entity<MenuItem>(entity =>
        {
            entity.HasKey(m => m.Id);
            entity.Property(m => m.Name).HasMaxLength(140).IsRequired();
            entity.Property(m => m.Description).HasMaxLength(500);
            entity.Property(m => m.Category).HasMaxLength(80).IsRequired();
            entity.Property(m => m.Price).HasPrecision(10, 2);
            entity.HasOne(m => m.Restaurant)
                .WithMany(r => r.MenuItems)
                .HasForeignKey(m => m.RestaurantId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Customer>(entity =>
        {
            entity.HasKey(c => c.Id);
            entity.Property(c => c.Name).HasMaxLength(120).IsRequired();
            entity.Property(c => c.Phone).HasMaxLength(32).IsRequired();
            entity.Property(c => c.Email).HasMaxLength(160).IsRequired();
            entity.Property(c => c.Address).HasMaxLength(300).IsRequired();
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(o => o.Id);
            entity.Property(o => o.Status).HasConversion<string>().HasMaxLength(40).IsRequired();
            entity.Property(o => o.TotalAmount).HasPrecision(10, 2);
            entity.HasOne(o => o.Customer)
                .WithMany(c => c.Orders)
                .HasForeignKey(o => o.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(o => o.Restaurant)
                .WithMany(r => r.Orders)
                .HasForeignKey(o => o.RestaurantId)
                .OnDelete(DeleteBehavior.Restrict);
            entity.HasIndex(o => o.CreatedAt);
        });

        modelBuilder.Entity<OrderItem>(entity =>
        {
            entity.HasKey(oi => oi.Id);
            entity.Property(oi => oi.UnitPrice).HasPrecision(10, 2);
            entity.Property(oi => oi.LineTotal).HasPrecision(10, 2);
            entity.HasOne(oi => oi.Order)
                .WithMany(o => o.Items)
                .HasForeignKey(oi => oi.OrderId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(oi => oi.MenuItem)
                .WithMany(m => m.OrderItems)
                .HasForeignKey(oi => oi.MenuItemId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<OrderStatusEvent>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.OldStatus).HasConversion<string>().HasMaxLength(40);
            entity.Property(e => e.NewStatus).HasConversion<string>().HasMaxLength(40).IsRequired();
            entity.Property(e => e.Note).HasMaxLength(300);
            entity.HasOne(e => e.Order)
                .WithMany(o => o.StatusEvents)
                .HasForeignKey(e => e.OrderId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasIndex(e => e.CreatedAt);
        });
    }
}
