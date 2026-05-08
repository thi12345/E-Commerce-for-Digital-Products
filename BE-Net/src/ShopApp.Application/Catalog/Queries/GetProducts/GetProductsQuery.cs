using MediatR;
using ShopApp.Application.Catalog.DTOs;
using ShopApp.Application.Common;
using ShopApp.Domain.Catalog.Enums;

namespace ShopApp.Application.Catalog.Queries.GetProducts;

public record GetProductsQuery(
    string? Name = null,
    Guid? CategoryId = null,
    decimal? MinRating = null,
    decimal? MinDiscountPercentage = null,
    ProductSortBy SortBy = ProductSortBy.Default,
    int Page = 1,
    int PageSize = 30) : IRequest<PagedResult<ProductDto>>;
