using ShopApp.Domain.Common;

namespace ShopApp.Domain.Orders.Events;

public sealed record OrderPlacedDomainEvent(
    Guid Id,
    Guid OrderId,
    Guid CustomerId,
    decimal TotalAmount,
    DateTime OccurredOn) : IDomainEvent;
