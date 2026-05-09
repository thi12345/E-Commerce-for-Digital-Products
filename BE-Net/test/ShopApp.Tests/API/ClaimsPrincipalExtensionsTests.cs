using System.Security.Claims;
using FluentAssertions;
using ShopApp.API.Auth;

namespace ShopApp.Tests.API;

public sealed class ClaimsPrincipalExtensionsTests
{
    [Fact]
    public void CanAccessUser_ShouldAllowSameUser()
    {
        var userId = Guid.NewGuid();
        var principal = CreatePrincipal(userId, AuthorizationPolicies.CustomerRole);

        principal.CanAccessUser(userId).Should().BeTrue();
    }

    [Fact]
    public void CanAccessUser_ShouldDenyDifferentCustomer()
    {
        var principal = CreatePrincipal(Guid.NewGuid(), AuthorizationPolicies.CustomerRole);

        principal.CanAccessUser(Guid.NewGuid()).Should().BeFalse();
    }

    [Fact]
    public void CanAccessUser_ShouldAllowAdminForAnyUser()
    {
        var principal = CreatePrincipal(Guid.NewGuid(), AuthorizationPolicies.AdminRole);

        principal.CanAccessUser(Guid.NewGuid()).Should().BeTrue();
    }

    private static ClaimsPrincipal CreatePrincipal(Guid userId, string role)
    {
        var identity = new ClaimsIdentity(
            [
                new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
                new Claim(ClaimTypes.Role, role)
            ],
            "Test");

        return new ClaimsPrincipal(identity);
    }
}
