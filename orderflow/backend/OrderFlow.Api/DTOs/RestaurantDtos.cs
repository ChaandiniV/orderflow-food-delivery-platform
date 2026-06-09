namespace OrderFlow.Api.DTOs;

public record RestaurantResponse(
    int Id,
    string Name,
    string Cuisine,
    string Area,
    decimal Rating,
    bool IsActive);

public record RestaurantDetailResponse(
    int Id,
    string Name,
    string Cuisine,
    string Area,
    decimal Rating,
    bool IsActive,
    IReadOnlyCollection<MenuItemResponse> MenuItems);

public record MenuItemResponse(
    int Id,
    int RestaurantId,
    string Name,
    string Description,
    decimal Price,
    string Category,
    bool IsAvailable);
