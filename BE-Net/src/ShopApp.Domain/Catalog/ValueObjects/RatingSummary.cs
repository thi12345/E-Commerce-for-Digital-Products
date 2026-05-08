using ShopApp.Domain.Common;
using ShopApp.Domain.Exceptions;

namespace ShopApp.Domain.Catalog.ValueObjects;

public sealed class RatingSummary : ValueObject
{
    public decimal Average { get; }
    public int TotalCount { get; }
    public int OneStarCount { get; }
    public int TwoStarCount { get; }
    public int ThreeStarCount { get; }
    public int FourStarCount { get; }
    public int FiveStarCount { get; }

    private RatingSummary(
        decimal average,
        int totalCount,
        int oneStarCount,
        int twoStarCount,
        int threeStarCount,
        int fourStarCount,
        int fiveStarCount)
    {
        Average = average;
        TotalCount = totalCount;
        OneStarCount = oneStarCount;
        TwoStarCount = twoStarCount;
        ThreeStarCount = threeStarCount;
        FourStarCount = fourStarCount;
        FiveStarCount = fiveStarCount;
    }

    public static RatingSummary Empty() => new(0, 0, 0, 0, 0, 0, 0);

    public static RatingSummary Create(
        int oneStarCount,
        int twoStarCount,
        int threeStarCount,
        int fourStarCount,
        int fiveStarCount)
    {
        if (oneStarCount < 0 || twoStarCount < 0 || threeStarCount < 0 || fourStarCount < 0 || fiveStarCount < 0)
            throw new DomainException("Rating counts cannot be negative.");

        var totalCount = oneStarCount + twoStarCount + threeStarCount + fourStarCount + fiveStarCount;
        if (totalCount == 0)
            return Empty();

        var totalScore = oneStarCount + twoStarCount * 2 + threeStarCount * 3 + fourStarCount * 4 + fiveStarCount * 5;
        var average = Math.Round(totalScore / (decimal)totalCount, 1);

        return new RatingSummary(average, totalCount, oneStarCount, twoStarCount, threeStarCount, fourStarCount, fiveStarCount);
    }

    public static RatingSummary FromAverage(decimal average, int totalCount)
    {
        if (average < 0 || average > 5)
            throw new DomainException("Rating average must be between 0 and 5.");

        if (totalCount < 0)
            throw new DomainException("Rating count cannot be negative.");

        if (totalCount == 0)
            return Empty();

        var roundedStar = Math.Clamp((int)Math.Round(average, MidpointRounding.AwayFromZero), 1, 5);
        return roundedStar switch
        {
            1 => Create(totalCount, 0, 0, 0, 0),
            2 => Create(0, totalCount, 0, 0, 0),
            3 => Create(0, 0, totalCount, 0, 0),
            4 => Create(0, 0, 0, totalCount, 0),
            _ => Create(0, 0, 0, 0, totalCount)
        };
    }

    public RatingSummary Add(decimal rating)
    {
        var star = ToStar(rating);
        return star switch
        {
            1 => Create(OneStarCount + 1, TwoStarCount, ThreeStarCount, FourStarCount, FiveStarCount),
            2 => Create(OneStarCount, TwoStarCount + 1, ThreeStarCount, FourStarCount, FiveStarCount),
            3 => Create(OneStarCount, TwoStarCount, ThreeStarCount + 1, FourStarCount, FiveStarCount),
            4 => Create(OneStarCount, TwoStarCount, ThreeStarCount, FourStarCount + 1, FiveStarCount),
            _ => Create(OneStarCount, TwoStarCount, ThreeStarCount, FourStarCount, FiveStarCount + 1)
        };
    }

    public RatingSummary Remove(decimal rating)
    {
        var star = ToStar(rating);
        return star switch
        {
            1 => Create(OneStarCount - 1, TwoStarCount, ThreeStarCount, FourStarCount, FiveStarCount),
            2 => Create(OneStarCount, TwoStarCount - 1, ThreeStarCount, FourStarCount, FiveStarCount),
            3 => Create(OneStarCount, TwoStarCount, ThreeStarCount - 1, FourStarCount, FiveStarCount),
            4 => Create(OneStarCount, TwoStarCount, ThreeStarCount, FourStarCount - 1, FiveStarCount),
            _ => Create(OneStarCount, TwoStarCount, ThreeStarCount, FourStarCount, FiveStarCount - 1)
        };
    }

    private static int ToStar(decimal rating)
    {
        if (rating < 1 || rating > 5)
            throw new DomainException("Rating must be between 1 and 5.");

        return Math.Clamp((int)Math.Round(rating, MidpointRounding.AwayFromZero), 1, 5);
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Average;
        yield return TotalCount;
        yield return OneStarCount;
        yield return TwoStarCount;
        yield return ThreeStarCount;
        yield return FourStarCount;
        yield return FiveStarCount;
    }
}
