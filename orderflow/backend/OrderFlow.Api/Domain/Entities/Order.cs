using OrderFlow.Api.Domain.Enums;

namespace OrderFlow.Api.Domain.Entities;

public class Order
{
    public int Id { get; set; }
    public int CustomerId { get; set; }
    public int RestaurantId { get; set; }
    public OrderStatus Status { get; set; } = OrderStatus.Placed;
    public decimal TotalAmount { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }

    public Customer? Customer { get; set; }
    public Restaurant? Restaurant { get; set; }
    public ICollection<OrderItem> Items { get; set; } = new List<OrderItem>();
    public ICollection<OrderStatusEvent> StatusEvents { get; set; } = new List<OrderStatusEvent>();
}
