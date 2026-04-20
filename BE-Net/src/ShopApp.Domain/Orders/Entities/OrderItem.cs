using ShopApp.Domain.Catalog.ValueObjects;
using ShopApp.Domain.Common;

namespace ShopApp.Domain.Orders.Entities;

public sealed class OrderItem : BaseEntity<Guid>
{
    public Guid OrderId { get; private set; }
    public Guid ProductId { get; private set; }
    public string ProductName { get; private set; } = string.Empty;
    public Money UnitPrice { get; private set; } = null!;
    public int Quantity { get; private set; }

    public Money TotalPrice => Money.Create(UnitPrice.Amount * Quantity, UnitPrice.Currency);

    private OrderItem() { }

    internal static OrderItem Create(Guid orderId, Guid productId, string productName, Money unitPrice, int quantity)
    {
        return new OrderItem
        {
            Id = Guid.NewGuid(),
            OrderId = orderId,
            ProductId = productId,
            ProductName = productName,
            UnitPrice = unitPrice,
            Quantity = quantity
        };
    }
}
