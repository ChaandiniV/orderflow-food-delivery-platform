namespace OrderFlow.Api.DTOs;

public record CreateCustomerRequest(
    string Name,
    string Phone,
    string Email,
    string Address);

public record CreateOrderItemRequest(
    int MenuItemId,
    int Quantity);

public record CreateOrderRequest(
    CreateCustomerRequest Customer,
    int RestaurantId,
    IReadOnlyCollection<CreateOrderItemRequest> Items);

public record UpdateOrderStatusRequest(
    string NewStatus,
    string? Note);

public record CustomerResponse(
    int Id,
    string Name,
    string Phone,
    string Email,
    string Address);

public record OrderItemResponse(
    int Id,
    int MenuItemId,
    string MenuItemName,
    int Quantity,
    decimal UnitPrice,
    decimal LineTotal);

public record OrderStatusEventResponse(
    int Id,
    int OrderId,
    string? OldStatus,
    string NewStatus,
    string Note,
    DateTimeOffset CreatedAt);

public record OrderResponse(
    int Id,
    CustomerResponse Customer,
    RestaurantResponse Restaurant,
    string Status,
    decimal TotalAmount,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt,
    IReadOnlyCollection<OrderItemResponse> Items,
    IReadOnlyCollection<OrderStatusEventResponse> StatusEvents);

public record OrderSummaryResponse(
    int Id,
    string CustomerName,
    string RestaurantName,
    string Status,
    decimal TotalAmount,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt);

public record ApiErrorResponse(
    string Message,
    int StatusCode,
    string? TraceId = null);
