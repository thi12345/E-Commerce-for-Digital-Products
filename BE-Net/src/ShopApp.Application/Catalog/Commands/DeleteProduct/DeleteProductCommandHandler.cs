using MediatR;
using Microsoft.Extensions.Logging;
using ShopApp.Application.Common.Interfaces;
using ShopApp.Domain.Catalog.Repositories;
using ShopApp.Domain.Exceptions;

namespace ShopApp.Application.Catalog.Commands.DeleteProduct;

public sealed class DeleteProductCommandHandler(
    IProductRepository productRepository,
    IUnitOfWork unitOfWork,
    ILogger<DeleteProductCommandHandler> logger)
    : IRequestHandler<DeleteProductCommand>
{
    public async Task Handle(DeleteProductCommand request, CancellationToken ct)
    {
        var product = await productRepository.GetByIdAsync(request.Id, ct)
            ?? throw new DomainException($"Product {request.Id} not found.");

        productRepository.Delete(product);
        await unitOfWork.SaveChangesAsync(ct);

        logger.LogInformation("Product deleted: Id={ProductId}", request.Id);
    }
}
