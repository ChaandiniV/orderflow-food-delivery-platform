using FluentAssertions;
using OrderFlow.Api.Domain.Enums;
using OrderFlow.Api.DTOs;
using OrderFlow.Api.Exceptions;
using OrderFlow.Api.Services;

namespace OrderFlow.Tests;

public class OrderServiceTests
{
    [Fact]
    public async Task CreateOrder_CalculatesCorrectTotal()
    {
        await using var dbContext = TestDbFactory.CreateContext(Guid.NewGuid().ToString());
        await TestDbFactory.SeedMenuAsync(dbContext);
        var service = CreateService(dbContext);

        var request = new CreateOrderRequest(
            new CreateCustomerRequest("Aisha Khan", "+971501112233", "aisha@example.com", "Dubai Marina, Dubai"),
            1,
            new List<CreateOrderItemRequest>
            {
                new(1, 2),
                new(2, 1)
            });

        var order = await service.CreateOrderAsync(request);

        order.TotalAmount.Should().Be(60m);
        order.Status.Should().Be(OrderStatus.Placed.ToString());
        order.Items.Should().HaveCount(2);
    }

    [Fact]
    public async Task CreateOrder_WithUnavailableMenuItem_IsRejected()
    {
        await using var dbContext = TestDbFactory.CreateContext(Guid.NewGuid().ToString());
        await TestDbFactory.SeedMenuAsync(dbContext);
        var service = CreateService(dbContext);

        var request = new CreateOrderRequest(
            new CreateCustomerRequest("Aisha Khan", "+971501112233", "aisha@example.com", "Dubai Marina, Dubai"),
            1,
            new List<CreateOrderItemRequest> { new(3, 1) });

        var act = async () => await service.CreateOrderAsync(request);

        await act.Should().ThrowAsync<BusinessRuleException>()
            .WithMessage("*currently unavailable*");
    }

    [Fact]
    public async Task UpdateOrderStatus_WithValidTransition_UpdatesOrderAndCreatesEvent()
    {
        await using var dbContext = TestDbFactory.CreateContext(Guid.NewGuid().ToString());
        await TestDbFactory.SeedMenuAsync(dbContext);
        var service = CreateService(dbContext);

        var order = await service.CreateOrderAsync(new CreateOrderRequest(
            new CreateCustomerRequest("Aisha Khan", "+971501112233", "aisha@example.com", "Dubai Marina, Dubai"),
            1,
            new List<CreateOrderItemRequest> { new(1, 1) }));

        var updated = await service.UpdateOrderStatusAsync(order.Id, new UpdateOrderStatusRequest("Accepted", "Restaurant accepted the order"));

        updated.Status.Should().Be(OrderStatus.Accepted.ToString());
        updated.StatusEvents.Should().HaveCount(2);
        updated.StatusEvents.Last().OldStatus.Should().Be(OrderStatus.Placed.ToString());
        updated.StatusEvents.Last().NewStatus.Should().Be(OrderStatus.Accepted.ToString());
    }

    [Fact]
    public async Task UpdateOrderStatus_WithInvalidTransition_IsRejected()
    {
        await using var dbContext = TestDbFactory.CreateContext(Guid.NewGuid().ToString());
        await TestDbFactory.SeedMenuAsync(dbContext);
        var service = CreateService(dbContext);

        var order = await service.CreateOrderAsync(new CreateOrderRequest(
            new CreateCustomerRequest("Aisha Khan", "+971501112233", "aisha@example.com", "Dubai Marina, Dubai"),
            1,
            new List<CreateOrderItemRequest> { new(1, 1) }));

        var act = async () => await service.UpdateOrderStatusAsync(order.Id, new UpdateOrderStatusRequest("Delivered", "Skip lifecycle"));

        await act.Should().ThrowAsync<BusinessRuleException>()
            .WithMessage("Cannot move order from Placed to Delivered*");
    }

    private static OrderService CreateService(OrderFlow.Api.Data.OrderFlowDbContext dbContext)
    {
        return new OrderService(dbContext, new OrderStatusTransitionService());
    }
}
