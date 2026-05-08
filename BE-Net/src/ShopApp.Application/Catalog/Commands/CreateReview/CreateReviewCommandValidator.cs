using FluentValidation;

namespace ShopApp.Application.Catalog.Commands.CreateReview;

public sealed class CreateReviewCommandValidator : AbstractValidator<CreateReviewCommand>
{
    public CreateReviewCommandValidator()
    {
        RuleFor(x => x.ProductId).NotEmpty();
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.Rating).InclusiveBetween(1, 5);
        RuleFor(x => x.ReviewTitle).NotEmpty().MaximumLength(500);
        RuleFor(x => x.ReviewContent).NotEmpty().MaximumLength(5000);
    }
}
