using ShopApp.Application.Auth.DTOs;
using ShopApp.Domain.Users.Entities;

namespace ShopApp.Application.Common.Interfaces;

public interface IJwtTokenGenerator
{
    AuthTokenDto GenerateToken(User user);
}
