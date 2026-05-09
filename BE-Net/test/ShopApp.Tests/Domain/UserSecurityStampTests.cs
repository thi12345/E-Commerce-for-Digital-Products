using FluentAssertions;
using ShopApp.Domain.Users.Entities;

namespace ShopApp.Tests.Domain;

public sealed class UserSecurityStampTests
{
    [Fact]
    public void Create_ShouldInitializeSecurityStamp()
    {
        var user = User.Create("customer@example.com", "Customer", "hash");

        user.SecurityStamp.Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    public void ChangePassword_ShouldRotateSecurityStamp()
    {
        var user = User.Create("customer@example.com", "Customer", "old-hash");
        var originalSecurityStamp = user.SecurityStamp;

        user.ChangePassword("new-hash");

        user.PasswordHash.Should().Be("new-hash");
        user.SecurityStamp.Should().NotBe(originalSecurityStamp);
    }
}
