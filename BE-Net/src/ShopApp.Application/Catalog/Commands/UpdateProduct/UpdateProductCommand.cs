using MediatR;
using ShopApp.Application.Catalog.DTOs;

namespace ShopApp.Application.Catalog.Commands.UpdateProduct;

public record UpdateProductCommand(
    Guid Id,
    string Name,
    string Description,
    decimal Price,
    string Currency,
    string DownloadUrl) : IRequest<ProductDto>;
