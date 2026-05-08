using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ShopApp.Domain.Users.Entities;

namespace ShopApp.Infrastructure.Persistence.Configurations;

public sealed class AddressConfiguration : IEntityTypeConfiguration<Address>
{
    public void Configure(EntityTypeBuilder<Address> builder)
    {
        builder.HasKey(a => a.Id);

        builder.Property(a => a.FullName).HasMaxLength(200).IsRequired();
        builder.Property(a => a.PhoneNumber).HasMaxLength(20).IsRequired();
        builder.Property(a => a.AddressLine).HasMaxLength(500).IsRequired();
        builder.Property(a => a.City).HasMaxLength(100).IsRequired();
        builder.Property(a => a.Province).HasMaxLength(100).IsRequired();
        builder.Property(a => a.Country).HasMaxLength(100).IsRequired();
        builder.Property(a => a.PostalCode).HasMaxLength(20);

        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(a => a.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(a => a.UserId);
    }
}
