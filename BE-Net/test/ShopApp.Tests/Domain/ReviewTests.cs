using FluentAssertions;
using ShopApp.Domain.Catalog.Entities;
using ShopApp.Domain.Exceptions;

namespace ShopApp.Tests.Domain;

public sealed class ReviewTests
{
    private static readonly Guid ProductId = Guid.NewGuid();
    private static readonly Guid UserId = Guid.NewGuid();

    [Fact]
    public void Create_ShouldCreateReview_WithValidData()
    {
        var review = Review.Create(ProductId, UserId, 5, "Great product!");

        review.ProductId.Should().Be(ProductId);
        review.UserId.Should().Be(UserId);
        review.Rating.Should().Be(5);
        review.Comment.Should().Be("Great product!");
        review.Id.Should().NotBeEmpty();
    }

    [Theory]
    [InlineData(1)]
    [InlineData(3)]
    [InlineData(5)]
    public void Create_ShouldAccept_ValidRatings(int rating)
    {
        var act = () => Review.Create(ProductId, UserId, rating, "OK");

        act.Should().NotThrow();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(6)]
    [InlineData(-1)]
    public void Create_ShouldThrow_WhenRatingIsOutOfRange(int rating)
    {
        var act = () => Review.Create(ProductId, UserId, rating, "comment");

        act.Should().Throw<DomainException>().WithMessage("*between 1 and 5*");
    }

    [Fact]
    public void Update_ShouldChangeRatingAndComment()
    {
        var review = Review.Create(ProductId, UserId, 3, "Average");

        review.Update(5, "Actually great!");

        review.Rating.Should().Be(5);
        review.Comment.Should().Be("Actually great!");
        review.UpdatedAt.Should().NotBeNull();
    }

    [Fact]
    public void Update_ShouldThrow_WhenRatingIsInvalid()
    {
        var review = Review.Create(ProductId, UserId, 3, "OK");

        var act = () => review.Update(10, "comment");

        act.Should().Throw<DomainException>();
    }
}
