using OrderFlow.Api.DTOs;

namespace OrderFlow.Api.Services;

public interface IOrderService
{
    Task<OrderResponse> CreateOrderAsync(CreateOrderRequest request, CancellationToken cancellationToken = default);
    Task<OrderResponse> GetOrderByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<OrderSummaryResponse>> GetRecentOrdersAsync(CancellationToken cancellationToken = default);
    Task<OrderResponse> UpdateOrderStatusAsync(int id, UpdateOrderStatusRequest request, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<OrderStatusEventResponse>> GetOrderEventsAsync(int id, CancellationToken cancellationToken = default);
}
