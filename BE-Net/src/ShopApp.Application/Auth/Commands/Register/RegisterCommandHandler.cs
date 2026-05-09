using MediatR;
using Microsoft.Extensions.Logging;
using ShopApp.Application.Auth.DTOs;
using ShopApp.Application.Common.Interfaces;
using ShopApp.Domain.Exceptions;
using ShopApp.Domain.Users.Entities;
using ShopApp.Domain.Users.Repositories;

namespace ShopApp.Application.Auth.Commands.Register;

public sealed class RegisterCommandHandler(
    IUserRepository userRepository,
    IPasswordHasher passwordHasher,
    IJwtTokenGenerator jwtTokenGenerator,
    IUnitOfWork unitOfWork,
    ILogger<RegisterCommandHandler> logger)
    : IRequestHandler<RegisterCommand, AuthResultDto>
{
    public async Task<AuthResultDto> Handle(RegisterCommand request, CancellationToken ct)
    {
        var existing = await userRepository.GetByEmailAsync(request.Email, ct);
        if (existing is not null)
            throw new DomainException($"Email '{request.Email}' is already in use.");

        logger.LogInformation("Registering user: Email={Email}", request.Email);

        var passwordHash = passwordHasher.Hash(request.Password);
        var user = User.Create(request.Email, request.FullName, passwordHash);

        await userRepository.AddAsync(user, ct);
        await unitOfWork.SaveChangesAsync(ct);

        return new AuthResultDto(user.ToDto(), jwtTokenGenerator.GenerateToken(user));
    }
}
