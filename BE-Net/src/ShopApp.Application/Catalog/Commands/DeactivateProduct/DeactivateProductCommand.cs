using MediatR;
using ShopApp.Application.Catalog.DTOs;

namespace ShopApp.Application.Catalog.Commands.DeactivateProduct;

public record DeactivateProductCommand(Guid Id) : IRequest<ProductDto>;
