using MediatR;
using Microsoft.Extensions.Logging;
using ShopApp.Application.Common;
using ShopApp.Application.Users.DTOs;
using ShopApp.Domain.Users.Entities;
using ShopApp.Domain.Users.Repositories;

namespace ShopApp.Application.Users.Queries.GetUsers;

public sealed class GetUsersQueryHandler(
    IUserRepository userRepository,
    ILogger<GetUsersQueryHandler> logger)
    : IRequestHandler<GetUsersQuery, PagedResult<UserDto>>
{
    public async Task<PagedResult<UserDto>> Handle(GetUsersQuery request, CancellationToken ct)
    {
        var pageSize = Math.Clamp(request.PageSize, 1, 100);
        var page = Math.Max(request.Page, 1);

        logger.LogDebug("Fetching users page={Page} pageSize={PageSize}", page, pageSize);

        var (items, totalCount) = await userRepository.GetPagedAsync(
            request.Email, request.Role, request.IsActive, page, pageSize, ct);

        var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

        return new PagedResult<UserDto>(
            items.Select(ToDto).ToList().AsReadOnly(),
            totalCount, totalPages, page, pageSize);
    }

    private static UserDto ToDto(User u) =>
        new(u.Id, u.Email, u.FullName, u.Role.ToString(), u.IsActive, u.CreatedAt);
}
