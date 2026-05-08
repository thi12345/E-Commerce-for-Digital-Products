using ShopApp.Domain.Common;
using ShopApp.Domain.Exceptions;

namespace ShopApp.Domain.Catalog.Entities;

public sealed class VariantOption : BaseEntity<Guid>
{
    public Guid VariantId { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string Value { get; private set; } = string.Empty;

    private VariantOption() { }

    public static VariantOption Create(Guid variantId, string name, string value)
    {
        if (variantId == Guid.Empty)
            throw new DomainException("Variant id is required.");

        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Variant option name cannot be empty.");

        if (string.IsNullOrWhiteSpace(value))
            throw new DomainException("Variant option value cannot be empty.");

        return new VariantOption
        {
            Id = Guid.NewGuid(),
            VariantId = variantId,
            Name = name.Trim(),
            Value = value.Trim()
        };
    }
}
