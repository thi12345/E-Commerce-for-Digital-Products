using FluentAssertions;
using ShopApp.Domain.Catalog.Entities;
using ShopApp.Domain.Catalog.Enums;
using ShopApp.Domain.Catalog.Events;
using ShopApp.Domain.Exceptions;

namespace ShopApp.Tests.Domain;

public sealed class ProductTests
{
    [Fact]
    public void Create_ShouldCreateProduct_WithDraftStatus()
    {
        var product = Product.Create("Ebook C#", "Learn C#", 29.99m, "USD", "https://example.com/ebook.pdf");

        product.Name.Value.Should().Be("Ebook C#");
        product.PrimaryVariant.DiscountedPrice.Amount.Should().Be(29.99m);
        product.PrimaryVariant.DiscountedPrice.Currency.Should().Be("USD");
        product.Status.Should().Be(ProductStatus.Draft);
    }

    [Fact]
    public void Create_ShouldRaiseDomainEvent()
    {
        var product = Product.Create("Ebook C#", "Learn C#", 29.99m, "USD", "https://example.com/ebook.pdf");

        product.DomainEvents.Should().ContainSingle()
            .Which.Should().BeOfType<ProductCreatedDomainEvent>();
    }

    [Fact]
    public void Create_ShouldThrow_WhenNameIsEmpty()
    {
        var act = () => Product.Create("", "desc", 10m, "USD", "https://example.com");
        act.Should().Throw<DomainException>();
    }

    [Fact]
    public void Create_ShouldThrow_WhenPriceIsNegative()
    {
        var act = () => Product.Create("Ebook", "desc", -1m, "USD", "https://example.com");
        act.Should().Throw<DomainException>();
    }

    [Fact]
    public void Activate_ShouldSetStatusToActive()
    {
        var product = Product.Create("Ebook C#", "Learn C#", 29.99m, "USD", "https://example.com/ebook.pdf");

        product.Activate();

        product.Status.Should().Be(ProductStatus.Active);
    }

    [Fact]
    public void Activate_ShouldThrow_WhenDownloadUrlIsEmpty()
    {
        var product = Product.Create("Ebook C#", "Learn C#", 29.99m, "USD", "");

        var act = () => product.Activate();

        act.Should().Throw<DomainException>().WithMessage("*download URL*");
    }

    [Fact]
    public void Deactivate_ShouldSetStatusToInactive()
    {
        var product = Product.Create("Ebook C#", "Learn C#", 29.99m, "USD", "https://example.com/ebook.pdf");
        product.Activate();

        product.Deactivate();

        product.Status.Should().Be(ProductStatus.Inactive);
    }

    [Fact]
    public void Update_ShouldChangeProductDetails()
    {
        var product = Product.Create("Old Name", "Old desc", 10m, "USD", "https://old.com");

        product.Update("New Name", "New desc", 20m, "USD", "https://new.com");

        product.Name.Value.Should().Be("New Name");
        product.PrimaryVariant.DiscountedPrice.Amount.Should().Be(20m);
        product.UpdatedAt.Should().NotBeNull();
    }
}
