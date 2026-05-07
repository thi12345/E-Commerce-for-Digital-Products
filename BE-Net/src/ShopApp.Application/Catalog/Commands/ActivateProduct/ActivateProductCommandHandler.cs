using MediatR;
using ShopApp.Application.Catalog.DTOs;
using ShopApp.Application.Common.Interfaces;
using ShopApp.Domain.Catalog.Repositories;
using ShopApp.Domain.Exceptions;

namespace ShopApp.Application.Catalog.Commands.ActivateProduct;

public sealed class ActivateProductCommandHandler(
    IProductRepository productRepository,
    IUnitOfWork unitOfWork)
    : IRequestHandler<ActivateProductCommand, ProductDto>
{
    public async Task<ProductDto> Handle(ActivateProductCommand request, CancellationToken ct)
    {
        var product = await productRepository.GetByIdAsync(request.Id, ct)
            ?? throw new DomainException($"Product {request.Id} not found.");
        product.Activate();
        productRepository.Update(product);
        await unitOfWork.SaveChangesAsync(ct);
        return new ProductDto(
            product.Id, product.Name.Value, product.Description,
            product.Price.Amount, product.Price.Currency,
            product.DownloadUrl, product.Status.ToString(), product.CreatedAt);
    }
}
