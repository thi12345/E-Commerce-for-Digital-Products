using MediatR;
using ShopApp.Application.Common.Interfaces;
using ShopApp.Application.Users.DTOs;
using ShopApp.Domain.Users.Entities;
using ShopApp.Domain.Users.Enums;
using ShopApp.Domain.Users.Repositories;

namespace ShopApp.Application.Users.Commands.CreateUser;

public sealed class CreateUserCommandHandler(
    IUserRepository userRepository,
    IUnitOfWork unitOfWork)
    : IRequestHandler<CreateUserCommand, UserDto>
{
    public async Task<UserDto> Handle(CreateUserCommand request, CancellationToken ct)
    {
        var role = Enum.Parse<UserRole>(request.Role, ignoreCase: true);
        var user = User.Create(request.Name, request.Email, role);
        await userRepository.AddAsync(user, ct);
        await unitOfWork.SaveChangesAsync(ct);
        return new UserDto(user.Id, user.Name, user.Email, user.Role.ToString(), user.CreatedAt);
    }
}
