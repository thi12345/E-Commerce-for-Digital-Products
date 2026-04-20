using FluentAssertions;
using ShopApp.Domain.Catalog.ValueObjects;
using ShopApp.Domain.Exceptions;
using ShopApp.Domain.Orders.Entities;
using ShopApp.Domain.Orders.Enums;
using ShopApp.Domain.Orders.Events;

namespace ShopApp.Tests.Domain;

public sealed class OrderTests
{
    private static readonly Guid CustomerId = Guid.NewGuid();
    private static readonly Guid ProductId = Guid.NewGuid();
    private static readonly Money Price = Money.Create(29.99m, "USD");

    [Fact]
    public void Create_ShouldCreateOrder_WithPendingStatus()
    {
        var order = Order.Create(CustomerId);

        order.CustomerId.Should().Be(CustomerId);
        order.Status.Should().Be(OrderStatus.Pending);
        order.Items.Should().BeEmpty();
    }

    [Fact]
    public void AddItem_ShouldAddItemToOrder()
    {
        var order = Order.Create(CustomerId);

        order.AddItem(ProductId, "Ebook C#", Price, 1);

        order.Items.Should().ContainSingle();
        order.Items[0].ProductName.Should().Be("Ebook C#");
    }

    [Fact]
    public void AddItem_ShouldThrow_WhenDuplicateProduct()
    {
        var order = Order.Create(CustomerId);
        order.AddItem(ProductId, "Ebook C#", Price, 1);

        var act = () => order.AddItem(ProductId, "Ebook C#", Price, 1);

        act.Should().Throw<DomainException>().WithMessage("*already in order*");
    }

    [Fact]
    public void TotalAmount_ShouldSumAllItems()
    {
        var order = Order.Create(CustomerId);
        order.AddItem(ProductId, "Ebook C#", Money.Create(10m), 2);
        order.AddItem(Guid.NewGuid(), "Video Course", Money.Create(20m), 1);

        order.TotalAmount.Amount.Should().Be(40m);
    }

    [Fact]
    public void Place_ShouldRaiseOrderPlacedDomainEvent()
    {
        var order = Order.Create(CustomerId);
        order.AddItem(ProductId, "Ebook C#", Price, 1);

        order.Place();

        order.DomainEvents.Should().ContainSingle()
            .Which.Should().BeOfType<OrderPlacedDomainEvent>();
    }

    [Fact]
    public void Place_ShouldThrow_WhenOrderIsEmpty()
    {
        var order = Order.Create(CustomerId);

        var act = () => order.Place();

        act.Should().Throw<DomainException>().WithMessage("*empty order*");
    }

    [Fact]
    public void Cancel_ShouldSetStatusToCancelled()
    {
        var order = Order.Create(CustomerId);
        order.AddItem(ProductId, "Ebook C#", Price, 1);

        order.Cancel();

        order.Status.Should().Be(OrderStatus.Cancelled);
    }

    [Fact]
    public void Cancel_ShouldThrow_WhenAlreadyCancelled()
    {
        var order = Order.Create(CustomerId);
        order.Cancel();

        var act = () => order.Cancel();

        act.Should().Throw<DomainException>();
    }

    [Fact]
    public void MarkPaid_ShouldSetStatusToPaid()
    {
        var order = Order.Create(CustomerId);
        order.AddItem(ProductId, "Ebook C#", Price, 1);

        order.MarkPaid();

        order.Status.Should().Be(OrderStatus.Paid);
    }
}
