using ShopApp.Domain.Catalog.Enums;
using ShopApp.Domain.Catalog.Events;
using ShopApp.Domain.Catalog.ValueObjects;
using ShopApp.Domain.Common;
using ShopApp.Domain.Exceptions;

namespace ShopApp.Domain.Catalog.Entities;

public sealed class Product : AggregateRoot<Guid>
{
    private readonly List<Variant> _variants = [];

    public ProductName Name { get; private set; } = null!;
    public string AboutProduct { get; private set; } = string.Empty;
    public RatingSummary RatingSummary { get; private set; } = RatingSummary.Empty();
    public string ImgLink { get; private set; } = string.Empty;
    public int PurchaseCount { get; private set; }
    public ProductStatus Status { get; private set; }
    public Guid? CategoryId { get; private set; }
    public IReadOnlyList<Variant> Variants => _variants.AsReadOnly();

    public Variant PrimaryVariant => _variants.FirstOrDefault(v => v.IsDefault) ?? _variants.First();
    public decimal Rating => RatingSummary.Average;
    public int RatingCount => RatingSummary.TotalCount;

    private Product() { }

    public static Product Create(
        string name,
        string aboutProduct,
        string imgLink = "",
        decimal rating = 0,
        int ratingCount = 0,
        Guid? categoryId = null)
    {
        return new Product
        {
            Id = Guid.NewGuid(),
            Name = ProductName.Create(name),
            AboutProduct = aboutProduct,
            RatingSummary = RatingSummary.FromAverage(rating, ratingCount),
            ImgLink = imgLink,
            PurchaseCount = 0,
            Status = ProductStatus.Draft,
            CategoryId = categoryId
        };
    }

    public static Product Create(
        string name,
        string aboutProduct,
        decimal price,
        string currency,
        string downloadUrl) =>
        Create(
            name,
            aboutProduct,
            price,
            price,
            0,
            currency,
            string.Empty,
            string.Empty,
            downloadUrl);

    public static Product Create(
        string name,
        string aboutProduct,
        decimal actualPrice,
        decimal discountedPrice,
        decimal discountPercentage,
        string currency,
        string imgLink,
        string productLink,
        string downloadUrl = "",
        decimal rating = 0,
        int ratingCount = 0,
        Guid? categoryId = null)
    {
        var product = Create(name, aboutProduct, imgLink, rating, ratingCount, categoryId);

        product.AddVariant(
            "Default",
            actualPrice,
            discountedPrice,
            discountPercentage,
            currency,
            productLink,
            downloadUrl,
            isDefault: true,
            stock: 100,
            options: [("Edition", "Standard")]);

        product.RaiseDomainEvent(new ProductCreatedDomainEvent(
            Guid.NewGuid(), product.Id, name, actualPrice, DateTime.UtcNow));

        return product;
    }

    public void Update(
        string name,
        string aboutProduct,
        string imgLink,
        decimal rating = 0,
        int ratingCount = 0,
        Guid? categoryId = null)
    {
        Name = ProductName.Create(name);
        AboutProduct = aboutProduct;
        RatingSummary = RatingSummary.FromAverage(rating, ratingCount);
        ImgLink = imgLink;
        CategoryId = categoryId;
        SetUpdatedAt();
    }

    public void Update(
        string name,
        string aboutProduct,
        decimal price,
        string currency,
        string downloadUrl) =>
        Update(
            name,
            aboutProduct,
            price,
            price,
            0,
            currency,
            ImgLink,
            PrimaryVariant.ProductLink,
            downloadUrl,
            RatingSummary.Average,
            RatingSummary.TotalCount,
            CategoryId);

    public void Update(
        string name,
        string aboutProduct,
        decimal actualPrice,
        decimal discountedPrice,
        decimal discountPercentage,
        string currency,
        string imgLink,
        string productLink,
        string downloadUrl = "",
        decimal rating = 0,
        int ratingCount = 0,
        Guid? categoryId = null)
    {
        Name = ProductName.Create(name);
        AboutProduct = aboutProduct;
        RatingSummary = RatingSummary.FromAverage(rating, ratingCount);
        ImgLink = imgLink;
        CategoryId = categoryId;
        PrimaryVariant.Update(
            PrimaryVariant.Name,
            actualPrice,
            discountedPrice,
            discountPercentage,
            currency,
            productLink,
            downloadUrl,
            PrimaryVariant.Stock,
            PrimaryVariant.IsDefault);
        SetUpdatedAt();
    }

    public Variant AddVariant(
        string name,
        decimal actualPrice,
        decimal discountedPrice,
        decimal discountPercentage,
        string currency,
        string productLink,
        string downloadUrl = "",
        bool isDefault = false,
        int stock = 0,
        IEnumerable<(string Name, string Value)>? options = null)
    {
        if (isDefault)
            UnsetDefaultVariants();

        if (!_variants.Any())
            isDefault = true;

        var variant = Variant.Create(
            Id,
            name,
            actualPrice,
            discountedPrice,
            discountPercentage,
            currency,
            productLink,
            downloadUrl,
            stock,
            isDefault);

        foreach (var (optionName, optionValue) in options ?? [])
            variant.AddOption(optionName, optionValue);

        _variants.Add(variant);
        SetUpdatedAt();
        return variant;
    }

    public void ReplaceVariants(IEnumerable<Variant> variants)
    {
        var newVariants = variants.ToList();
        if (!newVariants.Any())
            throw new DomainException("Product must have at least one variant.");

        if (newVariants.Count(v => v.IsDefault) != 1)
        {
            foreach (var variant in newVariants)
                variant.RemoveDefault();

            newVariants[0].MakeDefault();
        }

        _variants.Clear();
        _variants.AddRange(newVariants);
        SetUpdatedAt();
    }

    public void Activate()
    {
        if (string.IsNullOrWhiteSpace(PrimaryVariant.DownloadUrl))
            throw new DomainException("Cannot activate a product without a download URL.");
        Status = ProductStatus.Active;
        SetUpdatedAt();
    }

    public void Deactivate()
    {
        Status = ProductStatus.Inactive;
        SetUpdatedAt();
    }

    public void AddRating(decimal rating)
    {
        RatingSummary = RatingSummary.Add(rating);
        SetUpdatedAt();
    }

    public void ChangeRating(decimal oldRating, decimal newRating)
    {
        RatingSummary = RatingSummary.Remove(oldRating).Add(newRating);
        SetUpdatedAt();
    }

    public void RemoveRating(decimal rating)
    {
        RatingSummary = RatingSummary.Remove(rating);
        SetUpdatedAt();
    }

    private void UnsetDefaultVariants()
    {
        foreach (var variant in _variants)
            variant.RemoveDefault();
    }
}
