using ShopApp.Domain.Catalog.ValueObjects;
using ShopApp.Domain.Common;
using ShopApp.Domain.Exceptions;
using ShopApp.Domain.Orders.Enums;
using ShopApp.Domain.Orders.Events;

namespace ShopApp.Domain.Orders.Entities;

public sealed class Order : AggregateRoot<Guid>
{
    private readonly List<OrderItem> _items = [];

    public Guid CustomerId { get; private set; }
    public OrderStatus Status { get; private set; }
    public IReadOnlyList<OrderItem> Items => _items.AsReadOnly();

    public Money TotalAmount
    {
        get
        {
            if (!_items.Any()) return Money.Create(0);
            var first = _items[0].TotalPrice;
            return _items.Skip(1).Aggregate(first, (sum, item) => sum.Add(item.TotalPrice));
        }
    }

    private Order() { }

    public static Order Create(Guid customerId)
    {
        return new Order
        {
            Id = Guid.NewGuid(),
            CustomerId = customerId,
            Status = OrderStatus.Pending
        };
    }

    public void AddItem(Guid productId, string productName, Money unitPrice, int quantity)
    {
        if (Status != OrderStatus.Pending)
            throw new DomainException("Cannot modify a non-pending order.");

        var existing = _items.FirstOrDefault(i => i.ProductId == productId);
        if (existing is not null)
            throw new DomainException("Product already in order.");

        _items.Add(OrderItem.Create(Id, productId, productName, unitPrice, quantity));
        SetUpdatedAt();
    }

    public void Place()
    {
        if (!_items.Any())
            throw new DomainException("Cannot place an empty order.");

        if (Status != OrderStatus.Pending)
            throw new DomainException("Order has already been placed.");

        RaiseDomainEvent(new OrderPlacedDomainEvent(
            Guid.NewGuid(), Id, CustomerId, TotalAmount.Amount, DateTime.UtcNow));
    }

    public void MarkPaid()
    {
        if (Status != OrderStatus.Pending)
            throw new DomainException("Order must be pending to mark as paid.");
        Status = OrderStatus.Paid;
        SetUpdatedAt();
    }

    public void Complete()
    {
        if (Status != OrderStatus.Paid)
            throw new DomainException("Order must be paid before completing.");
        Status = OrderStatus.Completed;
        SetUpdatedAt();
    }

    public void Cancel()
    {
        if (Status is OrderStatus.Completed or OrderStatus.Cancelled)
            throw new DomainException("Cannot cancel a completed or already-cancelled order.");
        Status = OrderStatus.Cancelled;
        SetUpdatedAt();
    }
}
