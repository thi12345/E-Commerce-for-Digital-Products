using ShopApp.Application.Common.Interfaces;

namespace ShopApp.Infrastructure.Persistence;

public sealed class UnitOfWork(AppDbContext dbContext) : IUnitOfWork
{
    public Task<int> SaveChangesAsync(CancellationToken ct = default) =>
        dbContext.SaveChangesAsync(ct);
}
