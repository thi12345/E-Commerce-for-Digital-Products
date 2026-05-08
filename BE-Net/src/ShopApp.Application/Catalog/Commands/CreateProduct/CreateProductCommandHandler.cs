using MediatR;
using Microsoft.Extensions.Logging;
using ShopApp.Application.Catalog.DTOs;
using ShopApp.Application.Common.Interfaces;
using ShopApp.Domain.Catalog.Entities;
using ShopApp.Domain.Catalog.Repositories;

namespace ShopApp.Application.Catalog.Commands.CreateProduct;

public sealed class CreateProductCommandHandler(
    IProductRepository productRepository,
    IUnitOfWork unitOfWork,
    ILogger<CreateProductCommandHandler> logger)
    : IRequestHandler<CreateProductCommand, ProductDto>
{
    public async Task<ProductDto> Handle(CreateProductCommand request, CancellationToken ct)
    {
        logger.LogInformation("Creating product: Name={Name}, VariantCount={VariantCount}",
            request.Name, request.Variants?.Count ?? 0);

        var product = Product.Create(
            request.Name,
            request.AboutProduct,
            request.ImgLink,
            request.Rating,
            request.RatingCount,
            request.CategoryId);

        product.ReplaceVariants(request.Variants!.Select(v => CreateVariant(product.Id, v)));

        await productRepository.AddAsync(product, ct);
        await unitOfWork.SaveChangesAsync(ct);

        logger.LogInformation("Product created: Id={ProductId}, Name={Name}", product.Id, product.Name.Value);

        return product.ToDto();
    }

    private static Variant CreateVariant(Guid productId, ProductVariantRequest request)
    {
        var variant = Variant.Create(
            productId,
            request.Name,
            request.ActualPrice,
            request.DiscountedPrice,
            request.DiscountPercentage,
            request.Currency,
            request.ProductLink,
            request.DownloadUrl,
            request.Stock,
            request.IsDefault);

        foreach (var option in request.Options ?? [])
            variant.AddOption(option.Name, option.Value);

        return variant;
    }
}
