using MediatR;
using ShopApp.Application.Payments.DTOs;

namespace ShopApp.Application.Payments.Queries.GetPaymentById;

public record GetPaymentByIdQuery(Guid Id) : IRequest<PaymentDto?>;
