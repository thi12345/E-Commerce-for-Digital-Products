using MediatR;
using ShopApp.Application.Auth.DTOs;

namespace ShopApp.Application.Auth.Commands.Register;

public record RegisterCommand(
    string Email,
    string FullName,
    string Password) : IRequest<AuthResultDto>;
