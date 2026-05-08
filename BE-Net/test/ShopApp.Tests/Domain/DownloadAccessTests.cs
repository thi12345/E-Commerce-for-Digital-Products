using FluentAssertions;
using ShopApp.Domain.Exceptions;
using ShopApp.Domain.Orders.Entities;

namespace ShopApp.Tests.Domain;

public sealed class DownloadAccessTests
{
    private static readonly Guid UserId = Guid.NewGuid();
    private static readonly Guid ProductId = Guid.NewGuid();
    private static readonly Guid OrderId = Guid.NewGuid();

    [Fact]
    public void Create_ShouldCreateDownloadAccess_WithZeroDownloads()
    {
        var access = DownloadAccess.Create(UserId, ProductId, OrderId, maxDownloads: 5);

        access.UserId.Should().Be(UserId);
        access.ProductId.Should().Be(ProductId);
        access.OrderId.Should().Be(OrderId);
        access.DownloadCount.Should().Be(0);
        access.MaxDownloads.Should().Be(5);
        access.IsValid().Should().BeTrue();
    }

    [Fact]
    public void Create_ShouldThrow_WhenMaxDownloadsIsZero()
    {
        var act = () => DownloadAccess.Create(UserId, ProductId, OrderId, maxDownloads: 0);

        act.Should().Throw<DomainException>();
    }

    [Fact]
    public void RecordDownload_ShouldIncrementDownloadCount()
    {
        var access = DownloadAccess.Create(UserId, ProductId, OrderId, maxDownloads: 3);

        access.RecordDownload();
        access.RecordDownload();

        access.DownloadCount.Should().Be(2);
        access.IsValid().Should().BeTrue();
    }

    [Fact]
    public void RecordDownload_ShouldThrow_WhenLimitReached()
    {
        var access = DownloadAccess.Create(UserId, ProductId, OrderId, maxDownloads: 2);
        access.RecordDownload();
        access.RecordDownload();

        var act = () => access.RecordDownload();

        act.Should().Throw<DomainException>().WithMessage("*limit*");
    }

    [Fact]
    public void RecordDownload_ShouldThrow_WhenExpired()
    {
        var access = DownloadAccess.Create(UserId, ProductId, OrderId,
            maxDownloads: 5, expiresAt: DateTime.UtcNow.AddDays(-1));

        var act = () => access.RecordDownload();

        act.Should().Throw<DomainException>().WithMessage("*expired*");
    }

    [Fact]
    public void IsValid_ShouldReturnFalse_WhenLimitReached()
    {
        var access = DownloadAccess.Create(UserId, ProductId, OrderId, maxDownloads: 1);
        access.RecordDownload();

        access.IsValid().Should().BeFalse();
    }

    [Fact]
    public void IsValid_ShouldReturnFalse_WhenExpired()
    {
        var access = DownloadAccess.Create(UserId, ProductId, OrderId,
            maxDownloads: 5, expiresAt: DateTime.UtcNow.AddDays(-1));

        access.IsValid().Should().BeFalse();
    }
}
