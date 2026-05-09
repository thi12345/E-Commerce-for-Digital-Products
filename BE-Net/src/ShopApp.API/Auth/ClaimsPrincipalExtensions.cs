using System.Security.Claims;

namespace ShopApp.API.Auth;

public static class ClaimsPrincipalExtensions
{
    public static Guid? GetUserId(this ClaimsPrincipal user)
    {
        var value = user.FindFirstValue(ClaimTypes.NameIdentifier);
        return Guid.TryParse(value, out var id) ? id : null;
    }

    public static bool IsAdmin(this ClaimsPrincipal user) =>
        user.IsInRole(AuthorizationPolicies.AdminRole);

    public static bool CanAccessUser(this ClaimsPrincipal user, Guid userId)
    {
        var currentUserId = user.GetUserId();
        return user.IsAdmin() || currentUserId == userId;
    }
}
