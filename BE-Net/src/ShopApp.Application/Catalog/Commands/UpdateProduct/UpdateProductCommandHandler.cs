using MediatR;
using Microsoft.Extensions.Logging;
using ShopApp.Application.Catalog.DTOs;
using ShopApp.Application.Common.Interfaces;
using ShopApp.Domain.Catalog.Repositories;
using ShopApp.Domain.Exceptions;

namespace ShopApp.Application.Catalog.Commands.UpdateProduct;

public sealed class UpdateProductCommandHandler(
    IProductRepository productRepository,
    IUnitOfWork unitOfWork,
    ILogger<UpdateProductCommandHandler> logger)
    : IRequestHandler<UpdateProductCommand, ProductDto>
{
    public async Task<ProductDto> Handle(UpdateProductCommand request, CancellationToken ct)
    {
        logger.LogInformation("Updating product: Id={ProductId}", request.Id);

        var product = await productRepository.GetByIdAsync(request.Id, ct)
            ?? throw new DomainException($"Product {request.Id} not found.");

        product.Update(request.Name, request.Description, request.Price, request.Currency, request.DownloadUrl);
        productRepository.Update(product);
        await unitOfWork.SaveChangesAsync(ct);

        logger.LogInformation("Product updated: Id={ProductId}, Name={Name}", product.Id, product.Name.Value);

        return new ProductDto(
            product.Id, product.Name.Value, product.Description,
            product.Price.Amount, product.Price.Currency,
            product.DownloadUrl, product.Status.ToString(), product.CreatedAt);
    }
}
