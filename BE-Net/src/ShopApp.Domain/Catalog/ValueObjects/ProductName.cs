using ShopApp.Domain.Common;
using ShopApp.Domain.Exceptions;

namespace ShopApp.Domain.Catalog.ValueObjects;

public sealed class ProductName : ValueObject
{
    public string Value { get; }

    private ProductName(string value) => Value = value;

    public static ProductName Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value)) throw new DomainException("Product name cannot be empty.");
        if (value.Length > 200) throw new DomainException("Product name cannot exceed 200 characters.");
        return new ProductName(value.Trim());
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString() => Value;
}
