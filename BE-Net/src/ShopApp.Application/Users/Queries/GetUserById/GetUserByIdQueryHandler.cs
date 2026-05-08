using MediatR;
using ShopApp.Application.Users.DTOs;
using ShopApp.Domain.Users.Entities;
using ShopApp.Domain.Users.Repositories;

namespace ShopApp.Application.Users.Queries.GetUserById;

public sealed class GetUserByIdQueryHandler(IUserRepository userRepository)
    : IRequestHandler<GetUserByIdQuery, UserDto?>
{
    public async Task<UserDto?> Handle(GetUserByIdQuery request, CancellationToken ct)
    {
        var user = await userRepository.GetByIdAsync(request.Id, ct);
        return user is null ? null : ToDto(user);
    }

    private static UserDto ToDto(User u) =>
        new(u.Id, u.Email, u.FullName, u.Role.ToString(), u.IsActive, u.CreatedAt);
}
