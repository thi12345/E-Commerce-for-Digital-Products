using MediatR;
using ShopApp.Application.Payments.DTOs;
using ShopApp.Domain.Payments.Enums;

namespace ShopApp.Application.Payments.Commands.CreatePayment;

public record CreatePaymentCommand(
    Guid OrderId,
    Guid UserId,
    decimal Amount,
    string Currency,
    PaymentMethod Method) : IRequest<PaymentDto>;
