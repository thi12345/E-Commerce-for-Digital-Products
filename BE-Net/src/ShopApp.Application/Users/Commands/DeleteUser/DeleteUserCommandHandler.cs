using MediatR;
using ShopApp.Application.Common.Interfaces;
using ShopApp.Domain.Exceptions;
using ShopApp.Domain.Users.Repositories;

namespace ShopApp.Application.Users.Commands.DeleteUser;

public sealed class DeleteUserCommandHandler(
    IUserRepository userRepository,
    IUnitOfWork unitOfWork)
    : IRequestHandler<DeleteUserCommand>
{
    public async Task Handle(DeleteUserCommand request, CancellationToken ct)
    {
        var user = await userRepository.GetByIdAsync(request.Id, ct)
            ?? throw new DomainException($"User {request.Id} not found.");
        userRepository.Delete(user);
        await unitOfWork.SaveChangesAsync(ct);
    }
}
