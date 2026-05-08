using MediatR;
using Microsoft.Extensions.Logging;
using ShopApp.Application.Common.Interfaces;
using ShopApp.Application.Users.DTOs;
using ShopApp.Domain.Exceptions;
using ShopApp.Domain.Users.Entities;
using ShopApp.Domain.Users.Repositories;

namespace ShopApp.Application.Users.Commands.CreateUser;

public sealed class CreateUserCommandHandler(
    IUserRepository userRepository,
    IPasswordHasher passwordHasher,
    IUnitOfWork unitOfWork,
    ILogger<CreateUserCommandHandler> logger)
    : IRequestHandler<CreateUserCommand, UserDto>
{
    public async Task<UserDto> Handle(CreateUserCommand request, CancellationToken ct)
    {
        var existing = await userRepository.GetByEmailAsync(request.Email, ct);
        if (existing is not null)
            throw new DomainException($"Email '{request.Email}' is already in use.");

        logger.LogInformation("Creating user: Email={Email}", request.Email);

        var passwordHash = passwordHasher.Hash(request.Password);
        var user = User.Create(request.Email, request.FullName, passwordHash);

        await userRepository.AddAsync(user, ct);
        await unitOfWork.SaveChangesAsync(ct);

        logger.LogInformation("User created: Id={UserId}, Email={Email}", user.Id, user.Email);

        return ToDto(user);
    }

    private static UserDto ToDto(User u) =>
        new(u.Id, u.Email, u.FullName, u.Role.ToString(), u.IsActive, u.CreatedAt);
}
