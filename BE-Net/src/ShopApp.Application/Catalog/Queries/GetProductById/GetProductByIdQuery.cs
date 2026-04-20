using MediatR;
using ShopApp.Application.Catalog.DTOs;

namespace ShopApp.Application.Catalog.Queries.GetProductById;

public record GetProductByIdQuery(Guid Id) : IRequest<ProductDto?>;
