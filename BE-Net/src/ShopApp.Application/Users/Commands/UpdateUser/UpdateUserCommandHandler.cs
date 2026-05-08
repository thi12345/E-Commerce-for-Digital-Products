using MediatR;
using Microsoft.Extensions.Logging;
using ShopApp.Application.Common.Interfaces;
using ShopApp.Application.Users.DTOs;
using ShopApp.Domain.Exceptions;
using ShopApp.Domain.Users.Entities;
using ShopApp.Domain.Users.Repositories;

namespace ShopApp.Application.Users.Commands.UpdateUser;

public sealed class UpdateUserCommandHandler(
    IUserRepository userRepository,
    IUnitOfWork unitOfWork,
    ILogger<UpdateUserCommandHandler> logger)
    : IRequestHandler<UpdateUserCommand, UserDto>
{
    public async Task<UserDto> Handle(UpdateUserCommand request, CancellationToken ct)
    {
        var user = await userRepository.GetByIdAsync(request.Id, ct)
            ?? throw new DomainException($"User '{request.Id}' not found.");

        logger.LogInformation("Updating user: Id={UserId}", request.Id);

        user.UpdateProfile(request.FullName);

        if (request.IsActive && !user.IsActive)
            user.Activate();
        else if (!request.IsActive && user.IsActive)
            user.Deactivate();

        if (request.PromoteToAdmin)
            user.PromoteToAdmin();

        userRepository.Update(user);
        await unitOfWork.SaveChangesAsync(ct);

        logger.LogInformation("User updated: Id={UserId}", user.Id);

        return ToDto(user);
    }

    private static UserDto ToDto(User u) =>
        new(u.Id, u.Email, u.FullName, u.Role.ToString(), u.IsActive, u.CreatedAt);
}
