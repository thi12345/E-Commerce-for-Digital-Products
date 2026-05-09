using ShopApp.Application.Users.DTOs;

namespace ShopApp.Application.Auth.DTOs;

public record AuthResultDto(
    UserDto User,
    AuthTokenDto Token);
