using MediatR;
using ShopApp.Application.Catalog.DTOs;

namespace ShopApp.Application.Catalog.Queries.GetProducts;

public record GetProductsQuery : IRequest<IReadOnlyList<ProductDto>>;
