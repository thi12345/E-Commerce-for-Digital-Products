using MediatR;
using ShopApp.Application.Catalog.DTOs;

namespace ShopApp.Application.Catalog.Commands.CreateReview;

public record CreateReviewCommand(
    Guid ProductId,
    Guid UserId,
    decimal Rating,
    string ReviewTitle,
    string ReviewContent) : IRequest<ReviewDto>;
