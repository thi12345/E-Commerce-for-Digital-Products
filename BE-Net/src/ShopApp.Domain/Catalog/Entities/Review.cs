using ShopApp.Domain.Common;
using ShopApp.Domain.Exceptions;

namespace ShopApp.Domain.Catalog.Entities;

public sealed class Review : BaseEntity<Guid>
{
    public Guid ProductId { get; private set; }
    public Guid UserId { get; private set; }
    public decimal Rating { get; private set; }
    public string ReviewTitle { get; private set; } = string.Empty;
    public string ReviewContent { get; private set; } = string.Empty;
    public string Comment => ReviewContent;

    private Review() { }

    public static Review Create(Guid productId, Guid userId, decimal rating, string comment) =>
        Create(productId, userId, rating, string.Empty, comment);

    public static Review Create(Guid productId, Guid userId, decimal rating, string reviewTitle, string reviewContent)
    {
        if (rating < 1 || rating > 5)
            throw new DomainException("Rating must be between 1 and 5.");

        return new Review
        {
            Id = Guid.NewGuid(),
            ProductId = productId,
            UserId = userId,
            Rating = rating,
            ReviewTitle = reviewTitle.Trim(),
            ReviewContent = reviewContent.Trim()
        };
    }

    public void Update(decimal rating, string reviewTitle, string reviewContent)
    {
        if (rating < 1 || rating > 5)
            throw new DomainException("Rating must be between 1 and 5.");

        Rating = rating;
        ReviewTitle = reviewTitle.Trim();
        ReviewContent = reviewContent.Trim();
        SetUpdatedAt();
    }

    public void Update(decimal rating, string comment) =>
        Update(rating, ReviewTitle, comment);
}
