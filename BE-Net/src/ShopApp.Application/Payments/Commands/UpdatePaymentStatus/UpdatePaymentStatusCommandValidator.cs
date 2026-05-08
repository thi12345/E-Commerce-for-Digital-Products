using FluentValidation;
using ShopApp.Domain.Payments.Enums;

namespace ShopApp.Application.Payments.Commands.UpdatePaymentStatus;

public sealed class UpdatePaymentStatusCommandValidator : AbstractValidator<UpdatePaymentStatusCommand>
{
    public UpdatePaymentStatusCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.TransactionId)
            .NotEmpty()
            .When(x => x.NewStatus == PaymentStatus.Completed)
            .WithMessage("TransactionId is required when marking a payment as Completed.");
    }
}
