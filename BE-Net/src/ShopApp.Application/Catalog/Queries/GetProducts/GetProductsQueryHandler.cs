using MediatR;
using Microsoft.Extensions.Logging;
using ShopApp.Application.Catalog.DTOs;
using ShopApp.Application.Common;
using ShopApp.Domain.Catalog.Entities;
using ShopApp.Domain.Catalog.Repositories;

namespace ShopApp.Application.Catalog.Queries.GetProducts;

public sealed class GetProductsQueryHandler(
    IProductRepository productRepository,
    ILogger<GetProductsQueryHandler> logger)
    : IRequestHandler<GetProductsQuery, PagedResult<ProductDto>>
{
    public async Task<PagedResult<ProductDto>> Handle(GetProductsQuery request, CancellationToken ct)
    {
        var pageSize = Math.Clamp(request.PageSize, 1, 100);
        var page     = Math.Max(request.Page, 1);

        logger.LogDebug(
            "Fetching products page={Page} pageSize={PageSize} name={Name} minRating={MinRating} minDiscount={MinDiscount} sortBy={SortBy}",
            page, pageSize, request.Name, request.MinRating, request.MinDiscountPercentage, request.SortBy);

        var (items, totalCount) = await productRepository.GetPagedAsync(
            request.Name, request.CategoryId, request.MinRating,
            request.MinDiscountPercentage, request.SortBy,
            page, pageSize, ct);

        var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

        logger.LogDebug("Returned {Count}/{Total} products", items.Count, totalCount);

        return new PagedResult<ProductDto>(
            items.Select(p => p.ToListDto()).ToList().AsReadOnly(),
            totalCount, totalPages, page, pageSize);
    }
}
