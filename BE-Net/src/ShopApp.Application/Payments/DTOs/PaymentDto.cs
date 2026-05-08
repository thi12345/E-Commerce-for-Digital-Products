namespace ShopApp.Application.Payments.DTOs;

public record PaymentDto(
    Guid Id,
    Guid OrderId,
    Guid UserId,
    decimal Amount,
    string Currency,
    string Method,
    string Status,
    string? TransactionId,
    DateTime? PaidAt,
    DateTime CreatedAt);
