using FluentValidation;
using ShopApp.Application.Catalog.Commands.CreateProduct;

namespace ShopApp.Application.Catalog.Commands.UpdateProduct;

public sealed class UpdateProductCommandValidator : AbstractValidator<UpdateProductCommand>
{
    public UpdateProductCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Rating).InclusiveBetween(0, 5);
        RuleForEach(x => x.Variants).SetValidator(new ProductVariantRequestValidator());
    }
}
