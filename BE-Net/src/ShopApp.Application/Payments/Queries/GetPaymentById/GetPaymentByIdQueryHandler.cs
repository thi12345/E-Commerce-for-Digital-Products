using MediatR;
using ShopApp.Application.Payments.DTOs;
using ShopApp.Domain.Payments.Entities;
using ShopApp.Domain.Payments.Repositories;

namespace ShopApp.Application.Payments.Queries.GetPaymentById;

public sealed class GetPaymentByIdQueryHandler(IPaymentRepository paymentRepository)
    : IRequestHandler<GetPaymentByIdQuery, PaymentDto?>
{
    public async Task<PaymentDto?> Handle(GetPaymentByIdQuery request, CancellationToken ct)
    {
        var payment = await paymentRepository.GetByIdAsync(request.Id, ct);
        return payment is null ? null : ToDto(payment);
    }

    private static PaymentDto ToDto(Payment p) =>
        new(p.Id, p.OrderId, p.UserId, p.Amount.Amount, p.Amount.Currency,
            p.Method.ToString(), p.Status.ToString(), p.TransactionId, p.PaidAt, p.CreatedAt);
}
