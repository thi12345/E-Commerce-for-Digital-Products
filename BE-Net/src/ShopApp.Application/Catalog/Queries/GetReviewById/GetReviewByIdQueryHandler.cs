using MediatR;
using Microsoft.Extensions.Logging;
using ShopApp.Application.Catalog.DTOs;
using ShopApp.Domain.Catalog.Repositories;

namespace ShopApp.Application.Catalog.Queries.GetReviewById;

public sealed class GetReviewByIdQueryHandler(
    IReviewRepository reviewRepository,
    ILogger<GetReviewByIdQueryHandler> logger)
    : IRequestHandler<GetReviewByIdQuery, ReviewDto?>
{
    public async Task<ReviewDto?> Handle(GetReviewByIdQuery request, CancellationToken ct)
    {
        var review = await reviewRepository.GetByIdAsync(request.Id, ct);

        if (review is null)
        {
            logger.LogWarning("Review not found: Id={ReviewId}", request.Id);
            return null;
        }

        return new ReviewDto(review.Id, review.ProductId, review.UserId, review.Rating, review.ReviewTitle, review.ReviewContent, review.CreatedAt);
    }
}
