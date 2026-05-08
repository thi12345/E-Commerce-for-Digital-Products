using MediatR;

namespace ShopApp.Application.Payments.Commands.DeletePayment;

public record DeletePaymentCommand(Guid Id) : IRequest;
