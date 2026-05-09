namespace ShopApp.Application.Auth.DTOs;

public record AuthTokenDto(
    string AccessToken,
    string TokenType,
    DateTime ExpiresAt);
