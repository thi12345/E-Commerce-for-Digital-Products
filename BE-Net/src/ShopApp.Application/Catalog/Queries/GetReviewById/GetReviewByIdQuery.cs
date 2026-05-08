using MediatR;
using ShopApp.Application.Catalog.DTOs;

namespace ShopApp.Application.Catalog.Queries.GetReviewById;

public record GetReviewByIdQuery(Guid Id) : IRequest<ReviewDto?>;
