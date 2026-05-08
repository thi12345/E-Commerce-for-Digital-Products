using MediatR;
using ShopApp.Application.Payments.DTOs;
using ShopApp.Domain.Payments.Enums;

namespace ShopApp.Application.Payments.Commands.UpdatePaymentStatus;

public record UpdatePaymentStatusCommand(
    Guid Id,
    PaymentStatus NewStatus,
    string? TransactionId = null) : IRequest<PaymentDto>;
