using MediatR;
using Microsoft.Extensions.Logging;
using ShopApp.Application.Common.Interfaces;
using ShopApp.Domain.Exceptions;
using ShopApp.Domain.Users.Repositories;

namespace ShopApp.Application.Users.Commands.DeleteUser;

public sealed class DeleteUserCommandHandler(
    IUserRepository userRepository,
    IUnitOfWork unitOfWork,
    ILogger<DeleteUserCommandHandler> logger)
    : IRequestHandler<DeleteUserCommand>
{
    public async Task Handle(DeleteUserCommand request, CancellationToken ct)
    {
        var user = await userRepository.GetByIdAsync(request.Id, ct)
            ?? throw new DomainException($"User '{request.Id}' not found.");

        logger.LogInformation("Deleting user: Id={UserId}", request.Id);

        userRepository.Delete(user);
        await unitOfWork.SaveChangesAsync(ct);

        logger.LogInformation("User deleted: Id={UserId}", request.Id);
    }
}
