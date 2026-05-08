using ShopApp.Domain.Common;
using ShopApp.Domain.Exceptions;

namespace ShopApp.Domain.Users.Entities;

public sealed class Address : BaseEntity<Guid>
{
    public Guid UserId { get; private set; }
    public string FullName { get; private set; } = string.Empty;
    public string PhoneNumber { get; private set; } = string.Empty;
    public string AddressLine { get; private set; } = string.Empty;
    public string City { get; private set; } = string.Empty;
    public string Province { get; private set; } = string.Empty;
    public string Country { get; private set; } = string.Empty;
    public string? PostalCode { get; private set; }
    public bool IsDefault { get; private set; }

    private Address() { }

    public static Address Create(
        Guid userId,
        string fullName,
        string phoneNumber,
        string addressLine,
        string city,
        string province,
        string country,
        string? postalCode = null)
    {
        if (string.IsNullOrWhiteSpace(fullName))
            throw new DomainException("Full name cannot be empty.");
        if (string.IsNullOrWhiteSpace(addressLine))
            throw new DomainException("Address line cannot be empty.");
        if (string.IsNullOrWhiteSpace(city))
            throw new DomainException("City cannot be empty.");

        return new Address
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            FullName = fullName.Trim(),
            PhoneNumber = phoneNumber.Trim(),
            AddressLine = addressLine.Trim(),
            City = city.Trim(),
            Province = province.Trim(),
            Country = country.Trim(),
            PostalCode = postalCode?.Trim(),
            IsDefault = false
        };
    }

    public void Update(
        string fullName,
        string phoneNumber,
        string addressLine,
        string city,
        string province,
        string country,
        string? postalCode)
    {
        if (string.IsNullOrWhiteSpace(fullName))
            throw new DomainException("Full name cannot be empty.");
        if (string.IsNullOrWhiteSpace(addressLine))
            throw new DomainException("Address line cannot be empty.");

        FullName = fullName.Trim();
        PhoneNumber = phoneNumber.Trim();
        AddressLine = addressLine.Trim();
        City = city.Trim();
        Province = province.Trim();
        Country = country.Trim();
        PostalCode = postalCode?.Trim();
        SetUpdatedAt();
    }

    public void SetAsDefault()
    {
        IsDefault = true;
        SetUpdatedAt();
    }

    public void ClearDefault()
    {
        IsDefault = false;
        SetUpdatedAt();
    }
}
