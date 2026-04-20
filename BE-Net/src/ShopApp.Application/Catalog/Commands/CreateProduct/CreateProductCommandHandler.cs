using MediatR;
using ShopApp.Application.Catalog.DTOs;
using ShopApp.Application.Common.Interfaces;
using ShopApp.Domain.Catalog.Entities;
using ShopApp.Domain.Catalog.Repositories;

namespace ShopApp.Application.Catalog.Commands.CreateProduct;

public sealed class CreateProductCommandHandler(
    IProductRepository productRepository,
    IUnitOfWork unitOfWork)
    : IRequestHandler<CreateProductCommand, ProductDto>
{
    public async Task<ProductDto> Handle(CreateProductCommand request, CancellationToken ct)
    {
        var product = Product.Create(
            request.Name,
            request.Description,
            request.Price,
            request.Currency,
            request.DownloadUrl);

        await productRepository.AddAsync(product, ct);
        await unitOfWork.SaveChangesAsync(ct);

        return ToDto(product);
    }

    private static ProductDto ToDto(Product p) => new(
        p.Id, p.Name.Value, p.Description, p.Price.Amount, p.Price.Currency,
        p.DownloadUrl, p.Status.ToString(), p.CreatedAt);
}
