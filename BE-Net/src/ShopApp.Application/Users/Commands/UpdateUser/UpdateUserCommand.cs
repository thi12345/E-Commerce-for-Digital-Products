using MediatR;
using ShopApp.Application.Users.DTOs;

namespace ShopApp.Application.Users.Commands.UpdateUser;

public record UpdateUserCommand(Guid Id, string Name, string Email, string Role) : IRequest<UserDto>;
