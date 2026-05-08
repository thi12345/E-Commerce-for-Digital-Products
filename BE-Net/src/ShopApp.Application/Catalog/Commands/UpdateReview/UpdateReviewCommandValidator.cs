using FluentValidation;

namespace ShopApp.Application.Catalog.Commands.UpdateReview;

public sealed class UpdateReviewCommandValidator : AbstractValidator<UpdateReviewCommand>
{
    public UpdateReviewCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Rating).InclusiveBetween(1, 5);
        RuleFor(x => x.ReviewTitle).NotEmpty().MaximumLength(500);
        RuleFor(x => x.ReviewContent).NotEmpty().MaximumLength(5000);
    }
}
