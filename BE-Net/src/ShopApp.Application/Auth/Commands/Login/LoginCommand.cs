using MediatR;
using ShopApp.Application.Auth.DTOs;

namespace ShopApp.Application.Auth.Commands.Login;

public record LoginCommand(
    string Email,
    string Password) : IRequest<AuthResultDto>;
