using MediatR;
using Microsoft.Extensions.Logging;
using ShopApp.Application.Catalog.DTOs;
using ShopApp.Domain.Catalog.Repositories;

namespace ShopApp.Application.Catalog.Queries.GetReviews;

public sealed class GetReviewsQueryHandler(
    IReviewRepository reviewRepository,
    ILogger<GetReviewsQueryHandler> logger)
    : IRequestHandler<GetReviewsQuery, IReadOnlyList<ReviewDto>>
{
    public async Task<IReadOnlyList<ReviewDto>> Handle(GetReviewsQuery request, CancellationToken ct)
    {
        var reviews = await reviewRepository.GetByProductIdAsync(request.ProductId, ct);

        logger.LogInformation("Retrieved {Count} reviews for ProductId={ProductId}", reviews.Count, request.ProductId);

        return reviews
            .Select(r => new ReviewDto(r.Id, r.ProductId, r.UserId, r.Rating, r.ReviewTitle, r.ReviewContent, r.CreatedAt))
            .ToList();
    }
}
