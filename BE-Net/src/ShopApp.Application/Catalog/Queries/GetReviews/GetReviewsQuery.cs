using MediatR;
using ShopApp.Application.Catalog.DTOs;

namespace ShopApp.Application.Catalog.Queries.GetReviews;

public record GetReviewsQuery(Guid ProductId) : IRequest<IReadOnlyList<ReviewDto>>;
