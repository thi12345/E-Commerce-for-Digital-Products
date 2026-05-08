using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ShopApp.Domain.Orders.Entities;

namespace ShopApp.Infrastructure.Persistence.Configurations;

public sealed class DownloadAccessConfiguration : IEntityTypeConfiguration<DownloadAccess>
{
    public void Configure(EntityTypeBuilder<DownloadAccess> builder)
    {
        builder.HasKey(d => d.Id);

        builder.Property(d => d.MaxDownloads).IsRequired();
        builder.Property(d => d.DownloadCount).IsRequired();

        builder.HasIndex(d => new { d.UserId, d.ProductId });
        builder.HasIndex(d => d.OrderId);
    }
}
