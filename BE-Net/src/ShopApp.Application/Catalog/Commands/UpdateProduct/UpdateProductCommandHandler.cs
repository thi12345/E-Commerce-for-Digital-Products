using MediatR;
using Microsoft.Extensions.Logging;
using ShopApp.Application.Catalog.Commands.CreateProduct;
using ShopApp.Application.Catalog.DTOs;
using ShopApp.Application.Common.Interfaces;
using ShopApp.Domain.Catalog.Entities;
using ShopApp.Domain.Catalog.Repositories;
using ShopApp.Domain.Exceptions;

namespace ShopApp.Application.Catalog.Commands.UpdateProduct;

public sealed class UpdateProductCommandHandler(
    IProductRepository productRepository,
    IUnitOfWork unitOfWork,
    ILogger<UpdateProductCommandHandler> logger)
    : IRequestHandler<UpdateProductCommand, ProductDto>
{
    public async Task<ProductDto> Handle(UpdateProductCommand request, CancellationToken ct)
    {
        logger.LogInformation("Updating product: Id={ProductId}", request.Id);

        var product = await productRepository.GetByIdAsync(request.Id, ct)
            ?? throw new DomainException($"Product {request.Id} not found.");

        product.Update(
            request.Name,
            request.AboutProduct,
            request.ImgLink,
            request.Rating,
            request.RatingCount,
            request.CategoryId);

        if (request.Variants is { Count: > 0 })
            product.ReplaceVariants(request.Variants.Select(v => CreateVariant(product.Id, v)));

        productRepository.Update(product);
        await unitOfWork.SaveChangesAsync(ct);

        logger.LogInformation("Product updated: Id={ProductId}, Name={Name}", product.Id, product.Name.Value);

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
