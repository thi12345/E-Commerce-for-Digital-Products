using MediatR;
using ShopApp.Application.Catalog.DTOs;

namespace ShopApp.Application.Catalog.Commands.UpdateReview;

public record UpdateReviewCommand(
    Guid Id,
    decimal Rating,
    string ReviewTitle,
    string ReviewContent) : IRequest<ReviewDto>;
