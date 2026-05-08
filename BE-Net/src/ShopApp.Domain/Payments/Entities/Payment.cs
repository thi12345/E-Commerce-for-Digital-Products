using ShopApp.Domain.Catalog.ValueObjects;
using ShopApp.Domain.Common;
using ShopApp.Domain.Exceptions;
using ShopApp.Domain.Payments.Enums;

namespace ShopApp.Domain.Payments.Entities;

public sealed class Payment : AggregateRoot<Guid>
{
    public Guid OrderId { get; private set; }
    public Guid UserId { get; private set; }
    public Money Amount { get; private set; } = null!;
    public PaymentMethod Method { get; private set; }
    public PaymentStatus Status { get; private set; }
    public string? TransactionId { get; private set; }
    public DateTime? PaidAt { get; private set; }

    private Payment() { }

    public static Payment Create(Guid orderId, Guid userId, decimal amount, string currency, PaymentMethod method)
    {
        return new Payment
        {
            Id = Guid.NewGuid(),
            OrderId = orderId,
            UserId = userId,
            Amount = Money.Create(amount, currency),
            Method = method,
            Status = PaymentStatus.Pending
        };
    }

    public void MarkCompleted(string transactionId)
    {
        if (Status != PaymentStatus.Pending)
            throw new DomainException("Only pending payments can be marked as completed.");

        Status = PaymentStatus.Completed;
        TransactionId = transactionId;
        PaidAt = DateTime.UtcNow;
        SetUpdatedAt();
    }

    public void MarkFailed()
    {
        if (Status != PaymentStatus.Pending)
            throw new DomainException("Only pending payments can be marked as failed.");

        Status = PaymentStatus.Failed;
        SetUpdatedAt();
    }

    public void Refund()
    {
        if (Status != PaymentStatus.Completed)
            throw new DomainException("Only completed payments can be refunded.");

        Status = PaymentStatus.Refunded;
        SetUpdatedAt();
    }
}
