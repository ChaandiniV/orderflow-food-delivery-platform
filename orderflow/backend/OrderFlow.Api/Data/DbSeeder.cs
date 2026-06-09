using Microsoft.EntityFrameworkCore;
using OrderFlow.Api.Domain.Entities;

namespace OrderFlow.Api.Data;

public static class DbSeeder
{
    public static async Task SeedAsync(OrderFlowDbContext dbContext)
    {
        await dbContext.Database.EnsureCreatedAsync();

        if (await dbContext.Restaurants.AnyAsync())
        {
            return;
        }

        var restaurants = new List<Restaurant>
        {
            new()
            {
                Name = "Marina Bites",
                Cuisine = "Burgers",
                Area = "Dubai Marina",
                Rating = 4.6m,
                IsActive = true,
                MenuItems = new List<MenuItem>
                {
                    new() { Name = "Classic Marina Burger", Description = "Angus beef patty, cheddar, lettuce, tomato, house sauce.", Price = 34.00m, Category = "Burgers", IsAvailable = true },
                    new() { Name = "Spicy Chicken Burger", Description = "Crispy chicken, jalapeños, slaw, spicy mayo.", Price = 31.00m, Category = "Burgers", IsAvailable = true },
                    new() { Name = "Loaded Fries", Description = "Fries with cheese sauce, herbs, and crispy onions.", Price = 18.00m, Category = "Sides", IsAvailable = true },
                    new() { Name = "Truffle Fries", Description = "Crispy fries with parmesan and truffle oil.", Price = 22.00m, Category = "Sides", IsAvailable = true },
                    new() { Name = "Vanilla Milkshake", Description = "Creamy vanilla shake with whipped cream.", Price = 19.00m, Category = "Drinks", IsAvailable = true }
                }
            },
            new()
            {
                Name = "Deira Kitchen",
                Cuisine = "Arabic",
                Area = "Deira",
                Rating = 4.4m,
                IsActive = true,
                MenuItems = new List<MenuItem>
                {
                    new() { Name = "Chicken Shawarma Plate", Description = "Grilled chicken shawarma, rice, pickles, garlic sauce.", Price = 29.00m, Category = "Mains", IsAvailable = true },
                    new() { Name = "Lamb Mandi", Description = "Slow-cooked lamb with fragrant mandi rice.", Price = 48.00m, Category = "Mains", IsAvailable = true },
                    new() { Name = "Hummus with Bread", Description = "Creamy hummus served with warm Arabic bread.", Price = 16.00m, Category = "Starters", IsAvailable = true },
                    new() { Name = "Fattoush Salad", Description = "Fresh greens, crispy bread, sumac dressing.", Price = 20.00m, Category = "Salads", IsAvailable = true },
                    new() { Name = "Mint Lemonade", Description = "Fresh lemon and mint cooler.", Price = 14.00m, Category = "Drinks", IsAvailable = true }
                }
            },
            new()
            {
                Name = "JLT Bowls",
                Cuisine = "Healthy",
                Area = "Jumeirah Lakes Towers",
                Rating = 4.7m,
                IsActive = true,
                MenuItems = new List<MenuItem>
                {
                    new() { Name = "Chicken Quinoa Bowl", Description = "Grilled chicken, quinoa, avocado, greens, lemon dressing.", Price = 42.00m, Category = "Bowls", IsAvailable = true },
                    new() { Name = "Salmon Poke Bowl", Description = "Salmon, rice, edamame, cucumber, sesame sauce.", Price = 55.00m, Category = "Bowls", IsAvailable = true },
                    new() { Name = "Vegan Power Bowl", Description = "Chickpeas, sweet potato, kale, tahini dressing.", Price = 38.00m, Category = "Bowls", IsAvailable = true },
                    new() { Name = "Greek Yogurt Parfait", Description = "Greek yogurt, berries, granola, honey.", Price = 24.00m, Category = "Desserts", IsAvailable = true },
                    new() { Name = "Green Detox Juice", Description = "Apple, celery, cucumber, spinach, lemon.", Price = 21.00m, Category = "Drinks", IsAvailable = true }
                }
            }
        };

        dbContext.Restaurants.AddRange(restaurants);
        await dbContext.SaveChangesAsync();
    }
}
