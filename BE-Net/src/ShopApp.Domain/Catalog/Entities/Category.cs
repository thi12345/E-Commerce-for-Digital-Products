using ShopApp.Domain.Common;
using ShopApp.Domain.Exceptions;

namespace ShopApp.Domain.Catalog.Entities;

public sealed class Category : BaseEntity<Guid>
{
    private readonly List<Product> _products = [];

    public string Name { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public IReadOnlyList<Product> Products => _products.AsReadOnly();

    private Category() { }

    public static Category Create(string name, string? description = null)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Category name cannot be empty.");

        return new Category
        {
            Id = Guid.NewGuid(),
            Name = name.Trim(),
            Description = description
        };
    }

    public void Update(string name, string? description)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Category name cannot be empty.");

        Name = name.Trim();
        Description = description;
        SetUpdatedAt();
    }
}
