using MediatR;
using Microsoft.Extensions.Logging;
using ShopApp.Application.Catalog.DTOs;
using ShopApp.Application.Common.Interfaces;
using ShopApp.Domain.Catalog.Entities;
using ShopApp.Domain.Catalog.Repositories;
using ShopApp.Domain.Exceptions;

namespace ShopApp.Application.Catalog.Commands.CreateReview;

public sealed class CreateReviewCommandHandler(
    IReviewRepository reviewRepository,
    IProductRepository productRepository,
    IUnitOfWork unitOfWork,
    ILogger<CreateReviewCommandHandler> logger)
    : IRequestHandler<CreateReviewCommand, ReviewDto>
{
    public async Task<ReviewDto> Handle(CreateReviewCommand request, CancellationToken ct)
    {
        var product = await productRepository.GetByIdAsync(request.ProductId, ct);
        if (product is null)
            throw new DomainException($"Product {request.ProductId} not found.");

        if (await reviewRepository.ExistsByUserAndProductAsync(request.UserId, request.ProductId, ct))
            throw new DomainException("User has already reviewed this product.");

        var review = Review.Create(request.ProductId, request.UserId, request.Rating, request.ReviewTitle, request.ReviewContent);
        product.AddRating(review.Rating);

        await reviewRepository.AddAsync(review, ct);
        productRepository.Update(product);
        await unitOfWork.SaveChangesAsync(ct);

        logger.LogInformation("Review created: Id={ReviewId}, ProductId={ProductId}", review.Id, review.ProductId);

        return new ReviewDto(review.Id, review.ProductId, review.UserId, review.Rating, review.ReviewTitle, review.ReviewContent, review.CreatedAt);
    }
}
