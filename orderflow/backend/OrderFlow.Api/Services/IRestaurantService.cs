using OrderFlow.Api.DTOs;

namespace OrderFlow.Api.Services;

public interface IRestaurantService
{
    Task<IReadOnlyCollection<RestaurantResponse>> GetActiveRestaurantsAsync(CancellationToken cancellationToken = default);
    Task<RestaurantDetailResponse> GetRestaurantByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<MenuItemResponse>> GetAvailableMenuAsync(int restaurantId, CancellationToken cancellationToken = default);
}
