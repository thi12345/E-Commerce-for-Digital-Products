using MediatR;
using Microsoft.Extensions.Logging;
using ShopApp.Application.Catalog.DTOs;
using ShopApp.Domain.Catalog.Repositories;

namespace ShopApp.Application.Catalog.Queries.GetProductById;

public sealed class GetProductByIdQueryHandler(
    IProductRepository productRepository,
    ILogger<GetProductByIdQueryHandler> logger)
    : IRequestHandler<GetProductByIdQuery, ProductDto?>
{
    public async Task<ProductDto?> Handle(GetProductByIdQuery request, CancellationToken ct)
    {
        logger.LogDebug("Fetching product: Id={ProductId}", request.Id);

        var p = await productRepository.GetByIdAsync(request.Id, ct);

        if (p is null)
        {
            logger.LogWarning("Product not found: Id={ProductId}", request.Id);
            return null;
        }

        logger.LogDebug("Product found: Id={ProductId}, Name={Name}", p.Id, p.Name.Value);

        return new ProductDto(p.Id, p.Name.Value, p.Description, p.Price.Amount, p.Price.Currency,
            p.DownloadUrl, p.Status.ToString(), p.CreatedAt);
    }
}
