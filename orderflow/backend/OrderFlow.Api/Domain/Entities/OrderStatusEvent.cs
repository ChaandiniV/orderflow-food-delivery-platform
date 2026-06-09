using OrderFlow.Api.Domain.Enums;

namespace OrderFlow.Api.Domain.Entities;

public class OrderStatusEvent
{
    public int Id { get; set; }
    public int OrderId { get; set; }
    public OrderStatus? OldStatus { get; set; }
    public OrderStatus NewStatus { get; set; }
    public string Note { get; set; } = string.Empty;
    public DateTimeOffset CreatedAt { get; set; }

    public Order? Order { get; set; }
}
