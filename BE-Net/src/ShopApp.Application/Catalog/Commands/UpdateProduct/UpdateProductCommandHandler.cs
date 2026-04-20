using MediatR;
using ShopApp.Application.Catalog.DTOs;
using ShopApp.Application.Common.Interfaces;
using ShopApp.Domain.Catalog.Repositories;
using ShopApp.Domain.Exceptions;

namespace ShopApp.Application.Catalog.Commands.UpdateProduct;

public sealed class UpdateProductCommandHandler(
    IProductRepository productRepository,
    IUnitOfWork unitOfWork)
    : IRequestHandler<UpdateProductCommand, ProductDto>
{
    public async Task<ProductDto> Handle(UpdateProductCommand request, CancellationToken ct)
    {
        var product = await productRepository.GetByIdAsync(request.Id, ct)
            ?? throw new DomainException($"Product {request.Id} not found.");

        product.Update(request.Name, request.Description, request.Price, request.Currency, request.DownloadUrl);
        productRepository.Update(product);
        await unitOfWork.SaveChangesAsync(ct);

        return new ProductDto(
            product.Id, product.Name.Value, product.Description,
            product.Price.Amount, product.Price.Currency,
            product.DownloadUrl, product.Status.ToString(), product.CreatedAt);
    }
}
