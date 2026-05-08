using MediatR;
using Microsoft.Extensions.Logging;
using ShopApp.Application.Common.Interfaces;
using ShopApp.Application.Payments.DTOs;
using ShopApp.Domain.Payments.Entities;
using ShopApp.Domain.Payments.Repositories;

namespace ShopApp.Application.Payments.Commands.CreatePayment;

public sealed class CreatePaymentCommandHandler(
    IPaymentRepository paymentRepository,
    IUnitOfWork unitOfWork,
    ILogger<CreatePaymentCommandHandler> logger)
    : IRequestHandler<CreatePaymentCommand, PaymentDto>
{
    public async Task<PaymentDto> Handle(CreatePaymentCommand request, CancellationToken ct)
    {
        logger.LogInformation("Creating payment: OrderId={OrderId}, UserId={UserId}, Amount={Amount} {Currency}",
            request.OrderId, request.UserId, request.Amount, request.Currency);

        var payment = Payment.Create(
            request.OrderId,
            request.UserId,
            request.Amount,
            request.Currency,
            request.Method);

        await paymentRepository.AddAsync(payment, ct);
        await unitOfWork.SaveChangesAsync(ct);

        logger.LogInformation("Payment created: Id={PaymentId}", payment.Id);

        return ToDto(payment);
    }

    private static PaymentDto ToDto(Payment p) =>
        new(p.Id, p.OrderId, p.UserId, p.Amount.Amount, p.Amount.Currency,
            p.Method.ToString(), p.Status.ToString(), p.TransactionId, p.PaidAt, p.CreatedAt);
}
