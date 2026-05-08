using MediatR;
using Microsoft.Extensions.Logging;
using ShopApp.Application.Common.Interfaces;
using ShopApp.Domain.Exceptions;
using ShopApp.Domain.Payments.Repositories;

namespace ShopApp.Application.Payments.Commands.DeletePayment;

public sealed class DeletePaymentCommandHandler(
    IPaymentRepository paymentRepository,
    IUnitOfWork unitOfWork,
    ILogger<DeletePaymentCommandHandler> logger)
    : IRequestHandler<DeletePaymentCommand>
{
    public async Task Handle(DeletePaymentCommand request, CancellationToken ct)
    {
        var payment = await paymentRepository.GetByIdAsync(request.Id, ct)
            ?? throw new DomainException($"Payment '{request.Id}' not found.");

        logger.LogInformation("Deleting payment: Id={PaymentId}", request.Id);

        paymentRepository.Delete(payment);
        await unitOfWork.SaveChangesAsync(ct);

        logger.LogInformation("Payment deleted: Id={PaymentId}", request.Id);
    }
}
