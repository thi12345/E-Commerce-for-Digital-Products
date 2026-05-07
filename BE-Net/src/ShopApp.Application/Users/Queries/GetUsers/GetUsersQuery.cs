using MediatR;
using ShopApp.Application.Users.DTOs;

namespace ShopApp.Application.Users.Queries.GetUsers;

public record GetUsersQuery : IRequest<IReadOnlyList<UserDto>>;
