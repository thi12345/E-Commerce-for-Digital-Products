using ShopApp.Domain.Catalog.Entities;

namespace ShopApp.Application.Catalog.DTOs;

public static class ProductDtoMapper
{
    public static ProductDto ToDto(this Product product)
    {
        return product.ToDto(product.Variants);
    }

    public static ProductDto ToListDto(this Product product)
    {
        var lowestPriceVariant = product.Variants
            .OrderBy(v => v.DiscountedPrice.Amount)
            .ThenBy(v => v.ActualPrice.Amount)
            .FirstOrDefault();

        return product.ToDto(lowestPriceVariant is null ? [] : [lowestPriceVariant]);
    }

    private static ProductDto ToDto(this Product product, IEnumerable<Variant> variants)
    {
        return new ProductDto(
            product.Id,
            product.Name.Value,
            product.AboutProduct,
            product.RatingSummary.Average,
            product.RatingSummary.TotalCount,
            new RatingSummaryDto(
                product.RatingSummary.Average,
                product.RatingSummary.TotalCount,
                product.RatingSummary.OneStarCount,
                product.RatingSummary.TwoStarCount,
                product.RatingSummary.ThreeStarCount,
                product.RatingSummary.FourStarCount,
                product.RatingSummary.FiveStarCount),
            product.ImgLink,
            product.Status.ToString(),
            product.CreatedAt,
            product.PurchaseCount,
            variants.Select(ToDto).ToList().AsReadOnly());
    }

    private static VariantDto ToDto(Variant variant) => new(
        variant.Id,
        variant.Name,
        variant.ActualPrice.Amount,
        variant.DiscountedPrice.Amount,
        variant.DiscountPercentage,
        variant.ActualPrice.Currency,
        variant.ProductLink,
        variant.DownloadUrl,
        variant.Stock,
        variant.IsDefault,
        variant.Options.Select(ToDto).ToList().AsReadOnly());

    private static VariantOptionDto ToDto(VariantOption option) => new(
        option.Id,
        option.Name,
        option.Value);
}
