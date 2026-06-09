using OrderFlow.Api.Domain.Enums;

namespace OrderFlow.Api.Services;

public interface IOrderStatusTransitionService
{
    bool CanTransition(OrderStatus currentStatus, OrderStatus newStatus);
    IReadOnlyCollection<OrderStatus> GetAllowedNextStatuses(OrderStatus currentStatus);
}
