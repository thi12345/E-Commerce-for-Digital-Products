using MediatR;
using ShopApp.Application.Users.DTOs;

namespace ShopApp.Application.Users.Commands.CreateUser;

public record CreateUserCommand(
    string Email,
    string FullName,
    string Password) : IRequest<UserDto>;
