using ShopApp.Domain.Common;

namespace ShopApp.Domain.Catalog.Events;

public sealed record ProductCreatedDomainEvent(
    Guid Id,
    Guid ProductId,
    string ProductName,
    decimal Price,
    DateTime OccurredOn) : IDomainEvent;
