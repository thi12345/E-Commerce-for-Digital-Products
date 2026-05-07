using MediatR;
using ShopApp.Application.Catalog.DTOs;

namespace ShopApp.Application.Catalog.Commands.ActivateProduct;

public record ActivateProductCommand(Guid Id) : IRequest<ProductDto>;
