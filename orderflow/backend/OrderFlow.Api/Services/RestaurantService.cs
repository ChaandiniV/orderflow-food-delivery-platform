using Microsoft.EntityFrameworkCore;
using OrderFlow.Api.Data;
using OrderFlow.Api.DTOs;
using OrderFlow.Api.Exceptions;

namespace OrderFlow.Api.Services;

public class RestaurantService : IRestaurantService
{
    private readonly OrderFlowDbContext _dbContext;

    public RestaurantService(OrderFlowDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyCollection<RestaurantResponse>> GetActiveRestaurantsAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.Restaurants
            .AsNoTracking()
            .Where(r => r.IsActive)
            .OrderByDescending(r => r.Rating)
            .ThenBy(r => r.Name)
            .Select(r => new RestaurantResponse(r.Id, r.Name, r.Cuisine, r.Area, r.Rating, r.IsActive))
            .ToListAsync(cancellationToken);
    }

    public async Task<RestaurantDetailResponse> GetRestaurantByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var restaurant = await _dbContext.Restaurants
            .AsNoTracking()
            .Include(r => r.MenuItems.Where(m => m.IsAvailable))
            .FirstOrDefaultAsync(r => r.Id == id && r.IsActive, cancellationToken);

        if (restaurant is null)
        {
            throw new NotFoundException($"Restaurant {id} was not found.");
        }

        restaurant.MenuItems = restaurant.MenuItems
            .OrderBy(m => m.Category)
            .ThenBy(m => m.Name)
            .ToList();

        return restaurant.ToDetailResponse();
    }

    public async Task<IReadOnlyCollection<MenuItemResponse>> GetAvailableMenuAsync(int restaurantId, CancellationToken cancellationToken = default)
    {
        var restaurantExists = await _dbContext.Restaurants
            .AsNoTracking()
            .AnyAsync(r => r.Id == restaurantId && r.IsActive, cancellationToken);

        if (!restaurantExists)
        {
            throw new NotFoundException($"Restaurant {restaurantId} was not found.");
        }

        return await _dbContext.MenuItems
            .AsNoTracking()
            .Where(m => m.RestaurantId == restaurantId && m.IsAvailable)
            .OrderBy(m => m.Category)
            .ThenBy(m => m.Name)
            .Select(m => new MenuItemResponse(m.Id, m.RestaurantId, m.Name, m.Description, m.Price, m.Category, m.IsAvailable))
            .ToListAsync(cancellationToken);
    }
}
