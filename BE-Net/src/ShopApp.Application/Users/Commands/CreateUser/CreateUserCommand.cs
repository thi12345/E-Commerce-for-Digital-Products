using MediatR;
using ShopApp.Application.Users.DTOs;

namespace ShopApp.Application.Users.Commands.CreateUser;

public record CreateUserCommand(string Name, string Email, string Role) : IRequest<UserDto>;
