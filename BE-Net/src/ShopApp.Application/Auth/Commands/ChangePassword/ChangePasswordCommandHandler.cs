using MediatR;
using Microsoft.Extensions.Logging;
using ShopApp.Application.Common.Interfaces;
using ShopApp.Domain.Exceptions;
using ShopApp.Domain.Users.Repositories;

namespace ShopApp.Application.Auth.Commands.ChangePassword;

public sealed class ChangePasswordCommandHandler(
    IUserRepository userRepository,
    IPasswordHasher passwordHasher,
    IUnitOfWork unitOfWork,
    ILogger<ChangePasswordCommandHandler> logger)
    : IRequestHandler<ChangePasswordCommand>
{
    public async Task Handle(ChangePasswordCommand request, CancellationToken ct)
    {
        var user = await userRepository.GetByIdAsync(request.UserId, ct);
        if (user is null)
            throw new DomainException("User not found.");

        if (!user.IsActive)
            throw new DomainException("User account is inactive.");

        if (!passwordHasher.Verify(request.CurrentPassword, user.PasswordHash))
            throw new DomainException("Current password is invalid.");

        user.ChangePassword(passwordHasher.Hash(request.NewPassword));
        userRepository.Update(user);
        await unitOfWork.SaveChangesAsync(ct);

        logger.LogInformation("User password changed and sessions revoked: Id={UserId}, Email={Email}", user.Id, user.Email);
    }
}
