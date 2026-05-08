using FluentValidation;

namespace ShopApp.Application.Catalog.Commands.CreateProduct;

public sealed class CreateProductCommandValidator : AbstractValidator<CreateProductCommand>
{
    public CreateProductCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Rating).InclusiveBetween(0, 5);
        RuleFor(x => x.Variants).NotEmpty();
        RuleForEach(x => x.Variants).SetValidator(new ProductVariantRequestValidator());
    }
}

public sealed class ProductVariantRequestValidator : AbstractValidator<ProductVariantRequest>
{
    public ProductVariantRequestValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.ActualPrice).GreaterThan(0);
        RuleFor(x => x.DiscountedPrice).GreaterThanOrEqualTo(0);
        RuleFor(x => x.DiscountPercentage).InclusiveBetween(0, 100);
        RuleFor(x => x.Currency).NotEmpty().Length(3);
        RuleFor(x => x.Stock).GreaterThanOrEqualTo(0);
        RuleForEach(x => x.Options).SetValidator(new ProductVariantOptionRequestValidator());
    }
}

public sealed class ProductVariantOptionRequestValidator : AbstractValidator<ProductVariantOptionRequest>
{
    public ProductVariantOptionRequestValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Value).NotEmpty().MaximumLength(200);
    }
}
