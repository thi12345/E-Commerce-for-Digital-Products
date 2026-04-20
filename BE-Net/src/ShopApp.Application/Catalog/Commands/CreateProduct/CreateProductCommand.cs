using MediatR;
using ShopApp.Application.Catalog.DTOs;

namespace ShopApp.Application.Catalog.Commands.CreateProduct;

public record CreateProductCommand(
    string Name,
    string Description,
    decimal Price,
    string Currency,
    string DownloadUrl) : IRequest<ProductDto>;
