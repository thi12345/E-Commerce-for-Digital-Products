using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ShopApp.Domain.Catalog.Entities;

namespace ShopApp.Infrastructure.Persistence.Configurations;

public sealed class VariantConfiguration : IEntityTypeConfiguration<Variant>
{
    public void Configure(EntityTypeBuilder<Variant> builder)
    {
        builder.HasKey(v => v.Id);

        builder.Property(v => v.ProductId).IsRequired();
        builder.Property(v => v.Name).HasMaxLength(200).IsRequired();
        builder.Property(v => v.ProductLink).HasMaxLength(1000);
        builder.Property(v => v.DownloadUrl).HasMaxLength(500);
        builder.Property(v => v.DiscountPercentage).HasColumnType("decimal(5,2)");
        builder.Property(v => v.Stock).IsRequired();
        builder.Property(v => v.IsDefault);

        builder.OwnsOne(v => v.ActualPrice, money =>
        {
            money.Property(m => m.Amount).HasColumnName("ActualPrice").HasColumnType("decimal(18,2)").IsRequired();
            money.Property(m => m.Currency).HasColumnName("Currency").HasMaxLength(3).IsRequired();
        });

        builder.OwnsOne(v => v.DiscountedPrice, money =>
        {
            money.Property(m => m.Amount).HasColumnName("DiscountedPrice").HasColumnType("decimal(18,2)").IsRequired();
            money.Property(m => m.Currency).HasColumnName("DiscountedCurrency").HasMaxLength(3).IsRequired();
        });

        builder.HasMany(v => v.Options)
            .WithOne()
            .HasForeignKey(o => o.VariantId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(v => v.Options)
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.HasIndex(v => new { v.ProductId, v.IsDefault });
    }
}
