using MediatR;
using ShopApp.Application.Catalog.DTOs;
using ShopApp.Domain.Catalog.Repositories;

namespace ShopApp.Application.Catalog.Queries.GetProductById;

public sealed class GetProductByIdQueryHandler(IProductRepository productRepository)
    : IRequestHandler<GetProductByIdQuery, ProductDto?>
{
    public async Task<ProductDto?> Handle(GetProductByIdQuery request, CancellationToken ct)
    {
        var p = await productRepository.GetByIdAsync(request.Id, ct);
        if (p is null) return null;

        return new ProductDto(p.Id, p.Name.Value, p.Description, p.Price.Amount, p.Price.Currency,
            p.DownloadUrl, p.Status.ToString(), p.CreatedAt);
    }
}
