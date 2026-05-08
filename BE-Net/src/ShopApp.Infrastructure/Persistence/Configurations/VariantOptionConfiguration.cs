using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ShopApp.Domain.Catalog.Entities;

namespace ShopApp.Infrastructure.Persistence.Configurations;

public sealed class VariantOptionConfiguration : IEntityTypeConfiguration<VariantOption>
{
    public void Configure(EntityTypeBuilder<VariantOption> builder)
    {
        builder.HasKey(o => o.Id);

        builder.Property(o => o.VariantId).IsRequired();
        builder.Property(o => o.Name).HasMaxLength(100).IsRequired();
        builder.Property(o => o.Value).HasMaxLength(200).IsRequired();

        builder.HasIndex(o => new { o.VariantId, o.Name }).IsUnique();
    }
}
