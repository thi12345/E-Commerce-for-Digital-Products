using ShopApp.Application.Users.DTOs;
using ShopApp.Domain.Users.Entities;

namespace ShopApp.Application.Auth;

internal static class AuthMapping
{
    public static UserDto ToDto(this User user) =>
        new(user.Id, user.Email, user.FullName, user.Role.ToString(), user.IsActive, user.CreatedAt);
}
