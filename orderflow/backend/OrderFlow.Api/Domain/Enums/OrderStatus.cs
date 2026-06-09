namespace OrderFlow.Api.Domain.Enums;

public enum OrderStatus
{
    Placed = 1,
    Accepted = 2,
    Preparing = 3,
    ReadyForPickup = 4,
    OutForDelivery = 5,
    Delivered = 6,
    Cancelled = 7
}
