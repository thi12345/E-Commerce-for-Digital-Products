using MediatR;
using ShopApp.Application.Users.DTOs;

namespace ShopApp.Application.Users.Commands.UpdateUser;

public record UpdateUserCommand(
    Guid Id,
    string FullName,
    bool IsActive,
    bool PromoteToAdmin) : IRequest<UserDto>;
