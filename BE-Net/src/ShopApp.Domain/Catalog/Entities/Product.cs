using ShopApp.Domain.Catalog.Enums;
using ShopApp.Domain.Catalog.Events;
using ShopApp.Domain.Catalog.ValueObjects;
using ShopApp.Domain.Common;
using ShopApp.Domain.Exceptions;

namespace ShopApp.Domain.Catalog.Entities;

public sealed class Product : AggregateRoot<Guid>
{
    public ProductName Name { get; private set; } = null!;
    public string Description { get; private set; } = string.Empty;
    public Money Price { get; private set; } = null!;
    public string DownloadUrl { get; private set; } = string.Empty;
    public ProductStatus Status { get; private set; }
    public Guid? CategoryId { get; private set; }

    private Product() { }

    public static Product Create(string name, string description, decimal price, string currency, string downloadUrl, Guid? categoryId = null)
    {
        var product = new Product
        {
            Id = Guid.NewGuid(),
            Name = ProductName.Create(name),
            Description = description,
            Price = Money.Create(price, currency),
            DownloadUrl = downloadUrl,
            Status = ProductStatus.Draft,
            CategoryId = categoryId
        };

        product.RaiseDomainEvent(new ProductCreatedDomainEvent(
            Guid.NewGuid(), product.Id, name, price, DateTime.UtcNow));

        return product;
    }

    public void Update(string name, string description, decimal price, string currency, string downloadUrl, Guid? categoryId = null)
    {
        Name = ProductName.Create(name);
        Description = description;
        Price = Money.Create(price, currency);
        DownloadUrl = downloadUrl;
        CategoryId = categoryId;
        SetUpdatedAt();
    }

    public void Activate()
    {
        if (string.IsNullOrWhiteSpace(DownloadUrl))
            throw new DomainException("Cannot activate a product without a download URL.");
        Status = ProductStatus.Active;
        SetUpdatedAt();
    }

    public void Deactivate()
    {
        Status = ProductStatus.Inactive;
        SetUpdatedAt();
    }
}
