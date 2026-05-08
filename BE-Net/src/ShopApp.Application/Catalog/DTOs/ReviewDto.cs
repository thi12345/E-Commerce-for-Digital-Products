namespace ShopApp.Application.Catalog.DTOs;

public record ReviewDto(
    Guid Id,
    Guid ProductId,
    Guid UserId,
    decimal Rating,
    string ReviewTitle,
    string ReviewContent,
    DateTime CreatedAt);
