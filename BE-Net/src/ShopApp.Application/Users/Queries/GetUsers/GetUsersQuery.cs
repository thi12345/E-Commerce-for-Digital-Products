using MediatR;
using ShopApp.Application.Common;
using ShopApp.Application.Users.DTOs;
using ShopApp.Domain.Users.Enums;

namespace ShopApp.Application.Users.Queries.GetUsers;

public record GetUsersQuery(
    string? Email = null,
    UserRole? Role = null,
    bool? IsActive = null,
    int Page = 1,
    int PageSize = 20) : IRequest<PagedResult<UserDto>>;
