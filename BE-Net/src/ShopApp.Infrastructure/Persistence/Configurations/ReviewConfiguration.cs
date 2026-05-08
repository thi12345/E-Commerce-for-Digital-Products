using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ShopApp.Domain.Catalog.Entities;

namespace ShopApp.Infrastructure.Persistence.Configurations;

public sealed class ReviewConfiguration : IEntityTypeConfiguration<Review>
{
    public void Configure(EntityTypeBuilder<Review> builder)
    {
        builder.HasKey(r => r.Id);

        builder.Property(r => r.Rating).HasColumnType("decimal(3,1)").IsRequired();
        builder.Property(r => r.ReviewTitle).HasMaxLength(500);
        builder.Property(r => r.ReviewContent).HasMaxLength(5000);

        builder.HasIndex(r => new { r.ProductId, r.UserId }).IsUnique();
    }
}
