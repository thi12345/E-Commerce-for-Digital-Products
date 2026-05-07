using FluentValidation;

namespace ShopApp.Application.Users.Commands.CreateUser;

public sealed class CreateUserCommandValidator : AbstractValidator<CreateUserCommand>
{
    public CreateUserCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Email).NotEmpty().EmailAddress().MaximumLength(300);
        RuleFor(x => x.Role).NotEmpty()
            .Must(r => r is "Customer" or "Admin")
            .WithMessage("Role must be 'Customer' or 'Admin'.");
    }
}
