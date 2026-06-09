using Microsoft.EntityFrameworkCore;
using OrderFlow.Api.Data;
using OrderFlow.Api.Domain.Entities;

namespace OrderFlow.Tests;

public static class TestDbFactory
{
    public static OrderFlowDbContext CreateContext(string databaseName)
    {
        var options = new DbContextOptionsBuilder<OrderFlowDbContext>()
            .UseInMemoryDatabase(databaseName)
            .Options;

        return new OrderFlowDbContext(options);
    }

    public static async Task SeedMenuAsync(OrderFlowDbContext dbContext)
    {
        var restaurant = new Restaurant
        {
            Id = 1,
            Name = "Test Kitchen",
            Cuisine = "Burgers",
            Area = "Dubai Marina",
            Rating = 4.5m,
            IsActive = true,
            MenuItems = new List<MenuItem>
            {
                new() { Id = 1, Name = "Burger", Description = "Classic burger", Price = 25m, Category = "Mains", IsAvailable = true },
                new() { Id = 2, Name = "Fries", Description = "Crispy fries", Price = 10m, Category = "Sides", IsAvailable = true },
                new() { Id = 3, Name = "Sold Out Shake", Description = "Unavailable shake", Price = 15m, Category = "Drinks", IsAvailable = false }
            }
        };

        dbContext.Restaurants.Add(restaurant);
        await dbContext.SaveChangesAsync();
    }
}
