using OrderFlow.Api.Domain.Entities;
using OrderFlow.Api.DTOs;

namespace OrderFlow.Api.Services;

public static class MappingExtensions
{
    public static RestaurantResponse ToResponse(this Restaurant restaurant) =>
        new(
            restaurant.Id,
            restaurant.Name,
            restaurant.Cuisine,
            restaurant.Area,
            restaurant.Rating,
            restaurant.IsActive);

    public static MenuItemResponse ToResponse(this MenuItem item) =>
        new(
            item.Id,
            item.RestaurantId,
            item.Name,
            item.Description,
            item.Price,
            item.Category,
            item.IsAvailable);

    public static RestaurantDetailResponse ToDetailResponse(this Restaurant restaurant) =>
        new(
            restaurant.Id,
            restaurant.Name,
            restaurant.Cuisine,
            restaurant.Area,
            restaurant.Rating,
            restaurant.IsActive,
            restaurant.MenuItems.Select(m => m.ToResponse()).ToList());

    public static CustomerResponse ToResponse(this Customer customer) =>
        new(customer.Id, customer.Name, customer.Phone, customer.Email, customer.Address);

    public static OrderItemResponse ToResponse(this OrderItem item) =>
        new(
            item.Id,
            item.MenuItemId,
            item.MenuItem?.Name ?? "Unknown item",
            item.Quantity,
            item.UnitPrice,
            item.LineTotal);

    public static OrderStatusEventResponse ToResponse(this OrderStatusEvent statusEvent) =>
        new(
            statusEvent.Id,
            statusEvent.OrderId,
            statusEvent.OldStatus?.ToString(),
            statusEvent.NewStatus.ToString(),
            statusEvent.Note,
            statusEvent.CreatedAt);

    public static OrderResponse ToResponse(this Order order)
    {
        if (order.Customer is null)
        {
            throw new InvalidOperationException("Order customer was not loaded.");
        }

        if (order.Restaurant is null)
        {
            throw new InvalidOperationException("Order restaurant was not loaded.");
        }

        return new OrderResponse(
            order.Id,
            order.Customer.ToResponse(),
            order.Restaurant.ToResponse(),
            order.Status.ToString(),
            order.TotalAmount,
            order.CreatedAt,
            order.UpdatedAt,
            order.Items.Select(i => i.ToResponse()).ToList(),
            order.StatusEvents.OrderBy(e => e.CreatedAt).Select(e => e.ToResponse()).ToList());
    }

    public static OrderSummaryResponse ToSummaryResponse(this Order order) =>
        new(
            order.Id,
            order.Customer?.Name ?? "Unknown customer",
            order.Restaurant?.Name ?? "Unknown restaurant",
            order.Status.ToString(),
            order.TotalAmount,
            order.CreatedAt,
            order.UpdatedAt);
}
