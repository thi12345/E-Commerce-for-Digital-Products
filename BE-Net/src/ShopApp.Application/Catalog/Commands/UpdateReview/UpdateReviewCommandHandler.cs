using MediatR;
using Microsoft.Extensions.Logging;
using ShopApp.Application.Catalog.DTOs;
using ShopApp.Application.Common.Interfaces;
using ShopApp.Domain.Catalog.Repositories;
using ShopApp.Domain.Exceptions;

namespace ShopApp.Application.Catalog.Commands.UpdateReview;

public sealed class UpdateReviewCommandHandler(
    IReviewRepository reviewRepository,
    IProductRepository productRepository,
    IUnitOfWork unitOfWork,
    ILogger<UpdateReviewCommandHandler> logger)
    : IRequestHandler<UpdateReviewCommand, ReviewDto>
{
    public async Task<ReviewDto> Handle(UpdateReviewCommand request, CancellationToken ct)
    {
        var review = await reviewRepository.GetByIdAsync(request.Id, ct)
            ?? throw new DomainException($"Review {request.Id} not found.");

        var product = await productRepository.GetByIdAsync(review.ProductId, ct)
            ?? throw new DomainException($"Product {review.ProductId} not found.");

        var oldRating = review.Rating;
        review.Update(request.Rating, request.ReviewTitle, request.ReviewContent);
        product.ChangeRating(oldRating, review.Rating);

        reviewRepository.Update(review);
        productRepository.Update(product);
        await unitOfWork.SaveChangesAsync(ct);

        logger.LogInformation("Review updated: Id={ReviewId}", review.Id);

        return new ReviewDto(review.Id, review.ProductId, review.UserId, review.Rating, review.ReviewTitle, review.ReviewContent, review.CreatedAt);
    }
}
