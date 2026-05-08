using MediatR;
using ShopApp.Application.Catalog.Commands.CreateProduct;
using ShopApp.Application.Catalog.DTOs;

namespace ShopApp.Application.Catalog.Commands.UpdateProduct;

public record UpdateProductCommand(
    Guid Id,
    string Name,
    string AboutProduct,
    string ImgLink,
    decimal Rating = 0,
    int RatingCount = 0,
    Guid? CategoryId = null,
    IReadOnlyList<ProductVariantRequest>? Variants = null) : IRequest<ProductDto>;
