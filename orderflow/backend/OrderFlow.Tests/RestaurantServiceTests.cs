using FluentAssertions;
using OrderFlow.Api.Services;

namespace OrderFlow.Tests;

public class RestaurantServiceTests
{
    [Fact]
    public async Task GetAvailableMenu_ReturnsAvailableItemsOnly()
    {
        await using var dbContext = TestDbFactory.CreateContext(Guid.NewGuid().ToString());
        await TestDbFactory.SeedMenuAsync(dbContext);
        var service = new RestaurantService(dbContext);

        var menu = await service.GetAvailableMenuAsync(1);

        menu.Should().HaveCount(2);
        menu.Should().OnlyContain(item => item.IsAvailable);
        menu.Select(item => item.Name).Should().NotContain("Sold Out Shake");
    }
}
