using MediatR;
using ShopApp.Application.Common.Interfaces;
using ShopApp.Application.Users.DTOs;
using ShopApp.Domain.Exceptions;
using ShopApp.Domain.Users.Enums;
using ShopApp.Domain.Users.Repositories;

namespace ShopApp.Application.Users.Commands.UpdateUser;

public sealed class UpdateUserCommandHandler(
    IUserRepository userRepository,
    IUnitOfWork unitOfWork)
    : IRequestHandler<UpdateUserCommand, UserDto>
{
    public async Task<UserDto> Handle(UpdateUserCommand request, CancellationToken ct)
    {
        var user = await userRepository.GetByIdAsync(request.Id, ct)
            ?? throw new DomainException($"User {request.Id} not found.");
        var role = Enum.Parse<UserRole>(request.Role, ignoreCase: true);
        user.Update(request.Name, request.Email, role);
        userRepository.Update(user);
        await unitOfWork.SaveChangesAsync(ct);
        return new UserDto(user.Id, user.Name, user.Email, user.Role.ToString(), user.CreatedAt);
    }
}
