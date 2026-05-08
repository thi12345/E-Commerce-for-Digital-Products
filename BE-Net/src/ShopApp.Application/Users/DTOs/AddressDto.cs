namespace ShopApp.Application.Users.DTOs;

public record AddressDto(
    Guid Id,
    Guid UserId,
    string FullName,
    string PhoneNumber,
    string AddressLine,
    string City,
    string Province,
    string Country,
    string? PostalCode,
    bool IsDefault,
    DateTime CreatedAt);
