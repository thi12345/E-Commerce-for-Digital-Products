using MediatR;
using ShopApp.Application.Catalog.DTOs;
using ShopApp.Application.Common.Interfaces;
using ShopApp.Domain.Catalog.Repositories;
using ShopApp.Domain.Exceptions;

namespace ShopApp.Application.Catalog.Commands.DeactivateProduct;

public sealed class DeactivateProductCommandHandler(
    IProductRepository productRepository,
    IUnitOfWork unitOfWork)
    : IRequestHandler<DeactivateProductCommand, ProductDto>
{
    public async Task<ProductDto> Handle(DeactivateProductCommand request, CancellationToken ct)
    {
        var product = await productRepository.GetByIdAsync(request.Id, ct)
            ?? throw new DomainException($"Product {request.Id} not found.");
        product.Deactivate();
        productRepository.Update(product);
        await unitOfWork.SaveChangesAsync(ct);
        return new ProductDto(
            product.Id, product.Name.Value, product.Description,
            product.Price.Amount, product.Price.Currency,
            product.DownloadUrl, product.Status.ToString(), product.CreatedAt);
    }
}
