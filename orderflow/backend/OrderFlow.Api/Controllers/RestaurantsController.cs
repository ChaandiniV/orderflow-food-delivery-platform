using Microsoft.AspNetCore.Mvc;
using OrderFlow.Api.DTOs;
using OrderFlow.Api.Services;

namespace OrderFlow.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RestaurantsController : ControllerBase
{
    private readonly IRestaurantService _restaurantService;

    public RestaurantsController(IRestaurantService restaurantService)
    {
        _restaurantService = restaurantService;
    }

    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyCollection<RestaurantResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyCollection<RestaurantResponse>>> GetRestaurants(CancellationToken cancellationToken)
    {
        var restaurants = await _restaurantService.GetActiveRestaurantsAsync(cancellationToken);
        return Ok(restaurants);
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(RestaurantDetailResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<RestaurantDetailResponse>> GetRestaurant(int id, CancellationToken cancellationToken)
    {
        var restaurant = await _restaurantService.GetRestaurantByIdAsync(id, cancellationToken);
        return Ok(restaurant);
    }

    [HttpGet("{id:int}/menu")]
    [ProducesResponseType(typeof(IReadOnlyCollection<MenuItemResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IReadOnlyCollection<MenuItemResponse>>> GetMenu(int id, CancellationToken cancellationToken)
    {
        var menu = await _restaurantService.GetAvailableMenuAsync(id, cancellationToken);
        return Ok(menu);
    }
}
