using Microsoft.EntityFrameworkCore;
using OrderFlow.Api.Data;
using OrderFlow.Api.Domain.Entities;
using OrderFlow.Api.Domain.Enums;
using OrderFlow.Api.DTOs;
using OrderFlow.Api.Exceptions;

namespace OrderFlow.Api.Services;

public class OrderService : IOrderService
{
    private readonly OrderFlowDbContext _dbContext;
    private readonly IOrderStatusTransitionService _transitionService;

    public OrderService(OrderFlowDbContext dbContext, IOrderStatusTransitionService transitionService)
    {
        _dbContext = dbContext;
        _transitionService = transitionService;
    }

    public async Task<OrderResponse> CreateOrderAsync(CreateOrderRequest request, CancellationToken cancellationToken = default)
    {
        ValidateCreateOrderRequest(request);

        var restaurant = await _dbContext.Restaurants
            .FirstOrDefaultAsync(r => r.Id == request.RestaurantId && r.IsActive, cancellationToken);

        if (restaurant is null)
        {
            throw new NotFoundException($"Restaurant {request.RestaurantId} was not found.");
        }

        var groupedItems = request.Items
            .GroupBy(i => i.MenuItemId)
            .Select(g => new CreateOrderItemRequest(g.Key, g.Sum(i => i.Quantity)))
            .ToList();

        var requestedMenuItemIds = groupedItems.Select(i => i.MenuItemId).ToList();
        var menuItems = await _dbContext.MenuItems
            .Where(m => requestedMenuItemIds.Contains(m.Id))
            .ToListAsync(cancellationToken);

        var menuItemsById = menuItems.ToDictionary(m => m.Id);
        var orderItems = new List<OrderItem>();

        foreach (var itemRequest in groupedItems)
        {
            if (!menuItemsById.TryGetValue(itemRequest.MenuItemId, out var menuItem))
            {
                throw new BusinessRuleException($"Menu item {itemRequest.MenuItemId} was not found.");
            }

            if (menuItem.RestaurantId != restaurant.Id)
            {
                throw new BusinessRuleException($"Menu item {menuItem.Id} does not belong to restaurant {restaurant.Id}.");
            }

            if (!menuItem.IsAvailable)
            {
                throw new BusinessRuleException($"Menu item '{menuItem.Name}' is currently unavailable.");
            }

            var lineTotal = menuItem.Price * itemRequest.Quantity;
            orderItems.Add(new OrderItem
            {
                MenuItemId = menuItem.Id,
                Quantity = itemRequest.Quantity,
                UnitPrice = menuItem.Price,
                LineTotal = lineTotal
            });
        }

        var now = DateTimeOffset.UtcNow;
        var customer = new Customer
        {
            Name = request.Customer.Name.Trim(),
            Phone = request.Customer.Phone.Trim(),
            Email = request.Customer.Email.Trim(),
            Address = request.Customer.Address.Trim()
        };

        var order = new Order
        {
            Customer = customer,
            RestaurantId = restaurant.Id,
            Status = OrderStatus.Placed,
            CreatedAt = now,
            UpdatedAt = now,
            Items = orderItems,
            TotalAmount = orderItems.Sum(i => i.LineTotal),
            StatusEvents = new List<OrderStatusEvent>
            {
                new()
                {
                    OldStatus = null,
                    NewStatus = OrderStatus.Placed,
                    Note = "Order placed by customer",
                    CreatedAt = now
                }
            }
        };

        _dbContext.Orders.Add(order);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return await GetOrderByIdAsync(order.Id, cancellationToken);
    }

    public async Task<OrderResponse> GetOrderByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var order = await LoadOrderQuery()
            .AsNoTracking()
            .FirstOrDefaultAsync(o => o.Id == id, cancellationToken);

        if (order is null)
        {
            throw new NotFoundException($"Order {id} was not found.");
        }

        return order.ToResponse();
    }

    public async Task<IReadOnlyCollection<OrderSummaryResponse>> GetRecentOrdersAsync(CancellationToken cancellationToken = default)
    {
        var orders = await _dbContext.Orders
            .AsNoTracking()
            .Include(o => o.Customer)
            .Include(o => o.Restaurant)
            .OrderByDescending(o => o.CreatedAt)
            .Take(50)
            .ToListAsync(cancellationToken);

        return orders.Select(o => o.ToSummaryResponse()).ToList();
    }

    public async Task<OrderResponse> UpdateOrderStatusAsync(int id, UpdateOrderStatusRequest request, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(request.NewStatus))
        {
            throw new BusinessRuleException("New status is required.");
        }

        if (!Enum.TryParse<OrderStatus>(request.NewStatus, ignoreCase: true, out var newStatus))
        {
            throw new BusinessRuleException($"'{request.NewStatus}' is not a valid order status.");
        }

        var order = await _dbContext.Orders
            .FirstOrDefaultAsync(o => o.Id == id, cancellationToken);

        if (order is null)
        {
            throw new NotFoundException($"Order {id} was not found.");
        }

        if (!_transitionService.CanTransition(order.Status, newStatus))
        {
            var allowed = _transitionService.GetAllowedNextStatuses(order.Status);
            var allowedText = allowed.Any() ? string.Join(", ", allowed) : "no further statuses";
            throw new BusinessRuleException($"Cannot move order from {order.Status} to {newStatus}. Allowed next statuses: {allowedText}.");
        }

        var now = DateTimeOffset.UtcNow;
        var oldStatus = order.Status;
        order.Status = newStatus;
        order.UpdatedAt = now;

        _dbContext.OrderStatusEvents.Add(new OrderStatusEvent
        {
            OrderId = order.Id,
            OldStatus = oldStatus,
            NewStatus = newStatus,
            Note = string.IsNullOrWhiteSpace(request.Note) ? $"Status changed to {newStatus}" : request.Note.Trim(),
            CreatedAt = now
        });

        await _dbContext.SaveChangesAsync(cancellationToken);

        return await GetOrderByIdAsync(order.Id, cancellationToken);
    }

    public async Task<IReadOnlyCollection<OrderStatusEventResponse>> GetOrderEventsAsync(int id, CancellationToken cancellationToken = default)
    {
        var orderExists = await _dbContext.Orders
            .AsNoTracking()
            .AnyAsync(o => o.Id == id, cancellationToken);

        if (!orderExists)
        {
            throw new NotFoundException($"Order {id} was not found.");
        }

        var events = await _dbContext.OrderStatusEvents
            .AsNoTracking()
            .Where(e => e.OrderId == id)
            .OrderBy(e => e.CreatedAt)
            .ToListAsync(cancellationToken);

        return events.Select(e => e.ToResponse()).ToList();
    }

    private IQueryable<Order> LoadOrderQuery()
    {
        return _dbContext.Orders
            .Include(o => o.Customer)
            .Include(o => o.Restaurant)
            .Include(o => o.Items)
                .ThenInclude(i => i.MenuItem)
            .Include(o => o.StatusEvents)
            .AsSplitQuery();
    }

    private static void ValidateCreateOrderRequest(CreateOrderRequest request)
    {
        if (request.Customer is null)
        {
            throw new BusinessRuleException("Customer details are required.");
        }

        if (string.IsNullOrWhiteSpace(request.Customer.Name) ||
            string.IsNullOrWhiteSpace(request.Customer.Phone) ||
            string.IsNullOrWhiteSpace(request.Customer.Email) ||
            string.IsNullOrWhiteSpace(request.Customer.Address))
        {
            throw new BusinessRuleException("Customer name, phone, email, and address are required.");
        }

        if (request.RestaurantId <= 0)
        {
            throw new BusinessRuleException("RestaurantId must be a positive number.");
        }

        if (request.Items is null || request.Items.Count == 0)
        {
            throw new BusinessRuleException("At least one order item is required.");
        }

        if (request.Items.Any(i => i.MenuItemId <= 0 || i.Quantity <= 0))
        {
            throw new BusinessRuleException("Every order item must include a valid menuItemId and positive quantity.");
        }
    }
}
