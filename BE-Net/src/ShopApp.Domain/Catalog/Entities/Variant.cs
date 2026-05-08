using ShopApp.Domain.Catalog.ValueObjects;
using ShopApp.Domain.Common;
using ShopApp.Domain.Exceptions;

namespace ShopApp.Domain.Catalog.Entities;

public sealed class Variant : BaseEntity<Guid>
{
    private readonly List<VariantOption> _options = [];

    public Guid ProductId { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public Money ActualPrice { get; private set; } = null!;
    public Money DiscountedPrice { get; private set; } = null!;
    public decimal DiscountPercentage { get; private set; }
    public string ProductLink { get; private set; } = string.Empty;
    public string DownloadUrl { get; private set; } = string.Empty;
    public int Stock { get; private set; }
    public bool IsDefault { get; private set; }
    public IReadOnlyList<VariantOption> Options => _options.AsReadOnly();

    private Variant() { }

    public static Variant Create(
        Guid productId,
        string name,
        decimal actualPrice,
        decimal discountedPrice,
        decimal discountPercentage,
        string currency,
        string productLink,
        string downloadUrl = "",
        int stock = 0,
        bool isDefault = false)
    {
        if (productId == Guid.Empty)
            throw new DomainException("Product id is required.");

        var variant = new Variant
        {
            Id = Guid.NewGuid(),
            ProductId = productId,
            IsDefault = isDefault
        };

        variant.Update(name, actualPrice, discountedPrice, discountPercentage, currency, productLink, downloadUrl, stock, isDefault);
        return variant;
    }

    public void Update(
        string name,
        decimal actualPrice,
        decimal discountedPrice,
        decimal discountPercentage,
        string currency,
        string productLink,
        string downloadUrl = "",
        int stock = 0,
        bool isDefault = false)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Variant name cannot be empty.");

        if (name.Length > 200)
            throw new DomainException("Variant name cannot exceed 200 characters.");

        if (discountPercentage < 0 || discountPercentage > 100)
            throw new DomainException("Discount percentage must be between 0 and 100.");

        if (stock < 0)
            throw new DomainException("Variant stock cannot be negative.");

        Name = name.Trim();
        ActualPrice = Money.Create(actualPrice, currency);
        DiscountedPrice = Money.Create(discountedPrice, currency);
        DiscountPercentage = discountPercentage;
        ProductLink = productLink;
        DownloadUrl = downloadUrl;
        Stock = stock;
        IsDefault = isDefault;
        SetUpdatedAt();
    }

    public VariantOption AddOption(string name, string value)
    {
        if (_options.Any(o => o.Name.Equals(name, StringComparison.OrdinalIgnoreCase)))
            throw new DomainException($"Variant option '{name}' already exists.");

        var option = VariantOption.Create(Id, name, value);
        _options.Add(option);
        SetUpdatedAt();
        return option;
    }

    public void MakeDefault()
    {
        IsDefault = true;
        SetUpdatedAt();
    }

    public void RemoveDefault()
    {
        IsDefault = false;
        SetUpdatedAt();
    }
}
