using FluentAssertions;
using ShopApp.Domain.Exceptions;
using ShopApp.Domain.Users.Entities;
using ShopApp.Domain.Users.Enums;

namespace ShopApp.Tests.Domain;

public sealed class UserTests
{
    [Fact]
    public void Create_ShouldCreateUser_WithCustomerRole()
    {
        var user = User.Create("john@example.com", "John Doe", "hashed_password");

        user.Email.Should().Be("john@example.com");
        user.FullName.Should().Be("John Doe");
        user.Role.Should().Be(UserRole.Customer);
        user.IsActive.Should().BeTrue();
        user.Id.Should().NotBeEmpty();
    }

    [Fact]
    public void Create_ShouldNormalizeEmail_ToLowercase()
    {
        var user = User.Create("JOHN@EXAMPLE.COM", "John Doe", "hash");

        user.Email.Should().Be("john@example.com");
    }

    [Fact]
    public void Create_ShouldThrow_WhenEmailIsEmpty()
    {
        var act = () => User.Create("", "John Doe", "hash");

        act.Should().Throw<DomainException>();
    }

    [Fact]
    public void Create_ShouldThrow_WhenFullNameIsEmpty()
    {
        var act = () => User.Create("john@example.com", "", "hash");

        act.Should().Throw<DomainException>();
    }

    [Fact]
    public void Deactivate_ShouldSetIsActiveToFalse()
    {
        var user = User.Create("john@example.com", "John Doe", "hash");

        user.Deactivate();

        user.IsActive.Should().BeFalse();
        user.UpdatedAt.Should().NotBeNull();
    }

    [Fact]
    public void Activate_ShouldSetIsActiveToTrue()
    {
        var user = User.Create("john@example.com", "John Doe", "hash");
        user.Deactivate();

        user.Activate();

        user.IsActive.Should().BeTrue();
    }

    [Fact]
    public void PromoteToAdmin_ShouldChangeRoleToAdmin()
    {
        var user = User.Create("john@example.com", "John Doe", "hash");

        user.PromoteToAdmin();

        user.Role.Should().Be(UserRole.Admin);
        user.UpdatedAt.Should().NotBeNull();
    }

    [Fact]
    public void UpdateProfile_ShouldChangeFullName()
    {
        var user = User.Create("john@example.com", "John Doe", "hash");

        user.UpdateProfile("Jane Doe");

        user.FullName.Should().Be("Jane Doe");
    }

    [Fact]
    public void UpdateProfile_ShouldThrow_WhenFullNameIsEmpty()
    {
        var user = User.Create("john@example.com", "John Doe", "hash");

        var act = () => user.UpdateProfile("");

        act.Should().Throw<DomainException>();
    }
}
