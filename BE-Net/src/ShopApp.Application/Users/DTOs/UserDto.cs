namespace ShopApp.Application.Users.DTOs;

public record UserDto(
    Guid Id,
    string Email,
    string FullName,
    string Role,
    bool IsActive,
    DateTime CreatedAt);
