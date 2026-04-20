namespace ShopApp.Application.Catalog.DTOs;

public record ProductDto(
    Guid Id,
    string Name,
    string Description,
    decimal Price,
    string Currency,
    string DownloadUrl,
    string Status,
    DateTime CreatedAt);
