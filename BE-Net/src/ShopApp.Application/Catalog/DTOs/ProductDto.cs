namespace ShopApp.Application.Catalog.DTOs;

public record ProductDto(
    Guid Id,
    string Name,
    string AboutProduct,
    decimal Rating,
    int RatingCount,
    RatingSummaryDto RatingSummary,
    string ImgLink,
    string Status,
    DateTime CreatedAt,
    int PurchaseCount,
    IReadOnlyList<VariantDto>? Variants = null);

public record RatingSummaryDto(
    decimal Average,
    int TotalCount,
    int OneStarCount,
    int TwoStarCount,
    int ThreeStarCount,
    int FourStarCount,
    int FiveStarCount);

public record VariantDto(
    Guid Id,
    string Name,
    decimal ActualPrice,
    decimal DiscountedPrice,
    decimal DiscountPercentage,
    string Currency,
    string ProductLink,
    string DownloadUrl,
    int Stock,
    bool IsDefault,
    IReadOnlyList<VariantOptionDto> Options);

public record VariantOptionDto(
    Guid Id,
    string Name,
    string Value);
