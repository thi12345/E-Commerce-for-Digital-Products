using MediatR;
using Microsoft.Extensions.Logging;
using ShopApp.Application.Auth.DTOs;
using ShopApp.Application.Common.Interfaces;
using ShopApp.Domain.Exceptions;
using ShopApp.Domain.Users.Repositories;

namespace ShopApp.Application.Auth.Commands.Login;

public sealed class LoginCommandHandler(
    IUserRepository userRepository,
    IPasswordHasher passwordHasher,
    IJwtTokenGenerator jwtTokenGenerator,
    ILogger<LoginCommandHandler> logger)
    : IRequestHandler<LoginCommand, AuthResultDto>
{
    public async Task<AuthResultDto> Handle(LoginCommand request, CancellationToken ct)
    {
        var user = await userRepository.GetByEmailAsync(request.Email, ct);
        if (user is null || !passwordHasher.Verify(request.Password, user.PasswordHash))
            throw new DomainException("Invalid email or password.");

        if (!user.IsActive)
            throw new DomainException("User account is inactive.");

        logger.LogInformation("User logged in: Id={UserId}, Email={Email}", user.Id, user.Email);

        return new AuthResultDto(user.ToDto(), jwtTokenGenerator.GenerateToken(user));
    }
}
