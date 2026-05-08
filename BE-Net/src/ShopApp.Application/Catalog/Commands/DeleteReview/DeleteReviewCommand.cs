using MediatR;

namespace ShopApp.Application.Catalog.Commands.DeleteReview;

public record DeleteReviewCommand(Guid Id) : IRequest;
