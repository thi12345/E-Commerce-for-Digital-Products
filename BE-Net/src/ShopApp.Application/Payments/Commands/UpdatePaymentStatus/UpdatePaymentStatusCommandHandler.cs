using MediatR;
using Microsoft.Extensions.Logging;
using ShopApp.Application.Common.Interfaces;
using ShopApp.Application.Payments.DTOs;
using ShopApp.Domain.Exceptions;
using ShopApp.Domain.Payments.Entities;
using ShopApp.Domain.Payments.Enums;
using ShopApp.Domain.Payments.Repositories;

namespace ShopApp.Application.Payments.Commands.UpdatePaymentStatus;

public sealed class UpdatePaymentStatusCommandHandler(
    IPaymentRepository paymentRepository,
    IUnitOfWork unitOfWork,
    ILogger<UpdatePaymentStatusCommandHandler> logger)
    : IRequestHandler<UpdatePaymentStatusCommand, PaymentDto>
{
    public async Task<PaymentDto> Handle(UpdatePaymentStatusCommand request, CancellationToken ct)
    {
        var payment = await paymentRepository.GetByIdAsync(request.Id, ct)
            ?? throw new DomainException($"Payment '{request.Id}' not found.");

        logger.LogInformation("Updating payment status: Id={PaymentId}, NewStatus={Status}",
            request.Id, request.NewStatus);

        switch (request.NewStatus)
        {
            case PaymentStatus.Completed:
                payment.MarkCompleted(request.TransactionId!);
                break;
            case PaymentStatus.Failed:
                payment.MarkFailed();
                break;
            case PaymentStatus.Refunded:
                payment.Refund();
                break;
            default:
                throw new DomainException($"Cannot transition payment to status '{request.NewStatus}'.");
        }

        paymentRepository.Update(payment);
        await unitOfWork.SaveChangesAsync(ct);

        logger.LogInformation("Payment status updated: Id={PaymentId}, Status={Status}", payment.Id, payment.Status);

        return ToDto(payment);
    }

    private static PaymentDto ToDto(Payment p) =>
        new(p.Id, p.OrderId, p.UserId, p.Amount.Amount, p.Amount.Currency,
            p.Method.ToString(), p.Status.ToString(), p.TransactionId, p.PaidAt, p.CreatedAt);
}
