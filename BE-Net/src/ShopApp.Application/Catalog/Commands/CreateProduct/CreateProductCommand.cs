using MediatR;
using ShopApp.Application.Catalog.DTOs;

namespace ShopApp.Application.Catalog.Commands.CreateProduct;

public record CreateProductCommand(
    string Name,
    string AboutProduct,
    string ImgLink,
    decimal Rating = 0,
    int RatingCount = 0,
    Guid? CategoryId = null,
    IReadOnlyList<ProductVariantRequest>? Variants = null) : IRequest<ProductDto>
{
    public CreateProductCommand(
        string name,
        string aboutProduct,
        decimal price,
        string currency,
        string downloadUrl)
        : this(
            name,
            aboutProduct,
            string.Empty,
            Variants:
            [
                new ProductVariantRequest(
                    "Default",
                    price,
                    price,
                    0,
                    currency,
                    string.Empty,
                    downloadUrl,
                    0,
                    true)
            ])
    {
    }
}

public record ProductVariantRequest(
    string Name,
    decimal ActualPrice,
    decimal DiscountedPrice,
    decimal DiscountPercentage,
    string Currency,
    string ProductLink,
    string DownloadUrl = "",
    int Stock = 0,
    bool IsDefault = false,
    IReadOnlyList<ProductVariantOptionRequest>? Options = null);

public record ProductVariantOptionRequest(
    string Name,
    string Value);
