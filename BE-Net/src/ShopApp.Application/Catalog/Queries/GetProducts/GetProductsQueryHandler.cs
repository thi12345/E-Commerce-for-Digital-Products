using MediatR;
using Microsoft.Extensions.Logging;
using ShopApp.Application.Catalog.DTOs;
using ShopApp.Domain.Catalog.Entities;
using ShopApp.Domain.Catalog.Repositories;

namespace ShopApp.Application.Catalog.Queries.GetProducts;

public sealed class GetProductsQueryHandler(
    IProductRepository productRepository,
    ILogger<GetProductsQueryHandler> logger)
    : IRequestHandler<GetProductsQuery, IReadOnlyList<ProductDto>>
{
    public async Task<IReadOnlyList<ProductDto>> Handle(GetProductsQuery request, CancellationToken ct)
    {
        logger.LogDebug("Fetching all products");

        var products = await productRepository.GetAllAsync(ct);

        logger.LogDebug("Fetched {Count} products", products.Count);

        return products.Select(ToDto).ToList().AsReadOnly();
    }

    private static ProductDto ToDto(Product p) => new(
        p.Id, p.Name.Value, p.Description, p.Price.Amount, p.Price.Currency,
        p.DownloadUrl, p.Status.ToString(), p.CreatedAt);
}
