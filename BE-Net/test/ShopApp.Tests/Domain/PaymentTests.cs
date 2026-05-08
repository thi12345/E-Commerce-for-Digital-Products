using FluentAssertions;
using ShopApp.Domain.Exceptions;
using ShopApp.Domain.Payments.Entities;
using ShopApp.Domain.Payments.Enums;

namespace ShopApp.Tests.Domain;

public sealed class PaymentTests
{
    private static readonly Guid OrderId = Guid.NewGuid();
    private static readonly Guid UserId = Guid.NewGuid();

    [Fact]
    public void Create_ShouldCreatePayment_WithPendingStatus()
    {
        var payment = Payment.Create(OrderId, UserId, 99.99m, "USD", PaymentMethod.CreditCard);

        payment.OrderId.Should().Be(OrderId);
        payment.UserId.Should().Be(UserId);
        payment.Amount.Amount.Should().Be(99.99m);
        payment.Amount.Currency.Should().Be("USD");
        payment.Method.Should().Be(PaymentMethod.CreditCard);
        payment.Status.Should().Be(PaymentStatus.Pending);
        payment.TransactionId.Should().BeNull();
        payment.PaidAt.Should().BeNull();
    }

    [Fact]
    public void MarkCompleted_ShouldSetStatusAndTransactionId()
    {
        var payment = Payment.Create(OrderId, UserId, 99.99m, "USD", PaymentMethod.PayPal);

        payment.MarkCompleted("TXN-12345");

        payment.Status.Should().Be(PaymentStatus.Completed);
        payment.TransactionId.Should().Be("TXN-12345");
        payment.PaidAt.Should().NotBeNull();
        payment.UpdatedAt.Should().NotBeNull();
    }

    [Fact]
    public void MarkCompleted_ShouldThrow_WhenAlreadyCompleted()
    {
        var payment = Payment.Create(OrderId, UserId, 99.99m, "USD", PaymentMethod.CreditCard);
        payment.MarkCompleted("TXN-001");

        var act = () => payment.MarkCompleted("TXN-002");

        act.Should().Throw<DomainException>().WithMessage("*pending*");
    }

    [Fact]
    public void MarkFailed_ShouldSetStatusToFailed()
    {
        var payment = Payment.Create(OrderId, UserId, 99.99m, "USD", PaymentMethod.BankTransfer);

        payment.MarkFailed();

        payment.Status.Should().Be(PaymentStatus.Failed);
        payment.UpdatedAt.Should().NotBeNull();
    }

    [Fact]
    public void MarkFailed_ShouldThrow_WhenAlreadyCompleted()
    {
        var payment = Payment.Create(OrderId, UserId, 99.99m, "USD", PaymentMethod.CreditCard);
        payment.MarkCompleted("TXN-001");

        var act = () => payment.MarkFailed();

        act.Should().Throw<DomainException>();
    }

    [Fact]
    public void Refund_ShouldSetStatusToRefunded()
    {
        var payment = Payment.Create(OrderId, UserId, 99.99m, "USD", PaymentMethod.CreditCard);
        payment.MarkCompleted("TXN-001");

        payment.Refund();

        payment.Status.Should().Be(PaymentStatus.Refunded);
    }

    [Fact]
    public void Refund_ShouldThrow_WhenNotCompleted()
    {
        var payment = Payment.Create(OrderId, UserId, 99.99m, "USD", PaymentMethod.CreditCard);

        var act = () => payment.Refund();

        act.Should().Throw<DomainException>().WithMessage("*completed*");
    }
}
