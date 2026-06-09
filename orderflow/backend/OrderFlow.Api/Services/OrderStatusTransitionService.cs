using OrderFlow.Api.Domain.Enums;

namespace OrderFlow.Api.Services;

public class OrderStatusTransitionService : IOrderStatusTransitionService
{
    private static readonly IReadOnlyDictionary<OrderStatus, IReadOnlyCollection<OrderStatus>> AllowedTransitions =
        new Dictionary<OrderStatus, IReadOnlyCollection<OrderStatus>>
        {
            [OrderStatus.Placed] = new[] { OrderStatus.Accepted, OrderStatus.Cancelled },
            [OrderStatus.Accepted] = new[] { OrderStatus.Preparing, OrderStatus.Cancelled },
            [OrderStatus.Preparing] = new[] { OrderStatus.ReadyForPickup },
            [OrderStatus.ReadyForPickup] = new[] { OrderStatus.OutForDelivery },
            [OrderStatus.OutForDelivery] = new[] { OrderStatus.Delivered },
            [OrderStatus.Delivered] = Array.Empty<OrderStatus>(),
            [OrderStatus.Cancelled] = Array.Empty<OrderStatus>()
        };

    public bool CanTransition(OrderStatus currentStatus, OrderStatus newStatus)
    {
        return AllowedTransitions.TryGetValue(currentStatus, out var allowed) && allowed.Contains(newStatus);
    }

    public IReadOnlyCollection<OrderStatus> GetAllowedNextStatuses(OrderStatus currentStatus)
    {
        return AllowedTransitions.TryGetValue(currentStatus, out var allowed)
            ? allowed
            : Array.Empty<OrderStatus>();
    }
}
