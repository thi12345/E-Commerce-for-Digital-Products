using ShopApp.Domain.Common;
using ShopApp.Domain.Exceptions;
using ShopApp.Domain.Users.Enums;

namespace ShopApp.Domain.Users.Entities;

public sealed class User : AggregateRoot<Guid>
{
    public string Name { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;
    public UserRole Role { get; private set; }

    private User() { }

    public static User Create(string name, string email, UserRole role = UserRole.Customer)
    {
        if (string.IsNullOrWhiteSpace(name)) throw new DomainException("Name is required.");
        if (string.IsNullOrWhiteSpace(email)) throw new DomainException("Email is required.");
        return new User
        {
            Id = Guid.NewGuid(),
            Name = name.Trim(),
            Email = email.Trim().ToLowerInvariant(),
            Role = role
        };
    }

    public void Update(string name, string email, UserRole role)
    {
        if (string.IsNullOrWhiteSpace(name)) throw new DomainException("Name is required.");
        if (string.IsNullOrWhiteSpace(email)) throw new DomainException("Email is required.");
        Name = name.Trim();
        Email = email.Trim().ToLowerInvariant();
        Role = role;
        SetUpdatedAt();
    }
}
