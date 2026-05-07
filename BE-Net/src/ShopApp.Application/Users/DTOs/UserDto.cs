namespace ShopApp.Application.Users.DTOs;

public record UserDto(Guid Id, string Name, string Email, string Role, DateTime CreatedAt);
