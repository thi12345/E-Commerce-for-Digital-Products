using MediatR;
using Microsoft.Extensions.Logging;
using ShopApp.Application.Common.Interfaces;
using ShopApp.Domain.Catalog.Repositories;
using ShopApp.Domain.Exceptions;

namespace ShopApp.Application.Catalog.Commands.DeleteReview;

public sealed class DeleteReviewCommandHandler(
    IReviewRepository reviewRepository,
    IProductRepository productRepository,
    IUnitOfWork unitOfWork,
    ILogger<DeleteReviewCommandHandler> logger)
    : IRequestHandler<DeleteReviewCommand>
{
    public async Task Handle(DeleteReviewCommand request, CancellationToken ct)
    {
        var review = await reviewRepository.GetByIdAsync(request.Id, ct)
            ?? throw new DomainException($"Review {request.Id} not found.");

        var product = await productRepository.GetByIdAsync(review.ProductId, ct)
            ?? throw new DomainException($"Product {review.ProductId} not found.");

        product.RemoveRating(review.Rating);
        reviewRepository.Delete(review);
        productRepository.Update(product);
        await unitOfWork.SaveChangesAsync(ct);

        logger.LogInformation("Review deleted: Id={ReviewId}", request.Id);
    }
}
