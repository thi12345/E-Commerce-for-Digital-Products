using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ShopApp.Domain.Catalog.Entities;

namespace ShopApp.Infrastructure.Persistence.Configurations;

public sealed class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.HasKey(p => p.Id);

        builder.OwnsOne(p => p.Name, name =>
        {
            name.Property(n => n.Value)
                .HasColumnName("Name")
                .HasMaxLength(200)
                .IsRequired();
        });

        builder.OwnsOne(p => p.RatingSummary, rating =>
        {
            rating.Property(r => r.Average).HasColumnName("Rating").HasColumnType("decimal(3,1)");
            rating.Property(r => r.TotalCount).HasColumnName("RatingCount");
            rating.Property(r => r.OneStarCount).HasColumnName("OneStarCount");
            rating.Property(r => r.TwoStarCount).HasColumnName("TwoStarCount");
            rating.Property(r => r.ThreeStarCount).HasColumnName("ThreeStarCount");
            rating.Property(r => r.FourStarCount).HasColumnName("FourStarCount");
            rating.Property(r => r.FiveStarCount).HasColumnName("FiveStarCount");
        });
        builder.Property(p => p.AboutProduct).HasMaxLength(5000);
        builder.Property(p => p.ImgLink).HasMaxLength(1000);
        builder.Property(p => p.PurchaseCount);
        builder.Property(p => p.Status).HasConversion<string>().HasMaxLength(20);
        builder.Property(p => p.CategoryId);

        builder.HasMany(p => p.Variants)
            .WithOne()
            .HasForeignKey(v => v.ProductId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(p => p.Variants)
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.Ignore(p => p.PrimaryVariant);
        builder.Ignore(p => p.DomainEvents);
    }
}
