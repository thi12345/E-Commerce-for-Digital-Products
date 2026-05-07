using MediatR;
using ShopApp.Application.Users.DTOs;
using ShopApp.Domain.Users.Repositories;

namespace ShopApp.Application.Users.Queries.GetUsers;

public sealed class GetUsersQueryHandler(IUserRepository userRepository)
    : IRequestHandler<GetUsersQuery, IReadOnlyList<UserDto>>
{
    public async Task<IReadOnlyList<UserDto>> Handle(GetUsersQuery request, CancellationToken ct)
    {
        var users = await userRepository.GetAllAsync(ct);
        return users.Select(u => new UserDto(u.Id, u.Name, u.Email, u.Role.ToString(), u.CreatedAt))
                    .ToList().AsReadOnly();
    }
}
