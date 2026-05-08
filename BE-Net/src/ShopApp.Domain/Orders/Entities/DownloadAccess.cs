using ShopApp.Domain.Common;
using ShopApp.Domain.Exceptions;

namespace ShopApp.Domain.Orders.Entities;

public sealed class DownloadAccess : BaseEntity<Guid>
{
    public Guid UserId { get; private set; }
    public Guid ProductId { get; private set; }
    public Guid OrderId { get; private set; }
    public int DownloadCount { get; private set; }
    public int MaxDownloads { get; private set; }
    public DateTime? ExpiresAt { get; private set; }

    private DownloadAccess() { }

    public static DownloadAccess Create(Guid userId, Guid productId, Guid orderId, int maxDownloads = 5, DateTime? expiresAt = null)
    {
        if (maxDownloads <= 0)
            throw new DomainException("Max downloads must be greater than zero.");

        return new DownloadAccess
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            ProductId = productId,
            OrderId = orderId,
            DownloadCount = 0,
            MaxDownloads = maxDownloads,
            ExpiresAt = expiresAt
        };
    }

    public void RecordDownload()
    {
        if (ExpiresAt.HasValue && DateTime.UtcNow > ExpiresAt.Value)
            throw new DomainException("Download access has expired.");

        if (DownloadCount >= MaxDownloads)
            throw new DomainException("Download limit has been reached.");

        DownloadCount++;
        SetUpdatedAt();
    }

    public bool IsValid() =>
        DownloadCount < MaxDownloads &&
        (!ExpiresAt.HasValue || DateTime.UtcNow <= ExpiresAt.Value);
}
