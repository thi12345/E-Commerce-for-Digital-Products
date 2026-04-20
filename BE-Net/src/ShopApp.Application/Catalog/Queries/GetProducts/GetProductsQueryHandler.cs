using MediatR;
using ShopApp.Application.Catalog.DTOs;
using ShopApp.Domain.Catalog.Entities;
using ShopApp.Domain.Catalog.Repositories;

namespace ShopApp.Application.Catalog.Queries.GetProducts;

public sealed class GetProductsQueryHandler(IProductRepository productRepository)
    : IRequestHandler<GetProductsQuery, IReadOnlyList<ProductDto>>
{
    public async Task<IReadOnlyList<ProductDto>> Handle(GetProductsQuery request, CancellationToken ct)
    {
        var products = await productRepository.GetAllAsync(ct);
        return products.Select(ToDto).ToList().AsReadOnly();
    }

    private static ProductDto ToDto(Product p) => new(
        p.Id, p.Name.Value, p.Description, p.Price.Amount, p.Price.Currency,
        p.DownloadUrl, p.Status.ToString(), p.CreatedAt);
}
