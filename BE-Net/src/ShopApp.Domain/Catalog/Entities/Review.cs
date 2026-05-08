using ShopApp.Domain.Common;
using ShopApp.Domain.Exceptions;

namespace ShopApp.Domain.Catalog.Entities;

public sealed class Review : BaseEntity<Guid>
{
    public Guid ProductId { get; private set; }
    public Guid UserId { get; private set; }
    public int Rating { get; private set; }
    public string Comment { get; private set; } = string.Empty;

    private Review() { }

    public static Review Create(Guid productId, Guid userId, int rating, string comment)
    {
        if (rating < 1 || rating > 5)
            throw new DomainException("Rating must be between 1 and 5.");

        return new Review
        {
            Id = Guid.NewGuid(),
            ProductId = productId,
            UserId = userId,
            Rating = rating,
            Comment = comment.Trim()
        };
    }

    public void Update(int rating, string comment)
    {
        if (rating < 1 || rating > 5)
            throw new DomainException("Rating must be between 1 and 5.");

        Rating = rating;
        Comment = comment.Trim();
        SetUpdatedAt();
    }
}
