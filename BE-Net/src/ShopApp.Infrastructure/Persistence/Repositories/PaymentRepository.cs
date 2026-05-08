using Microsoft.EntityFrameworkCore;
using ShopApp.Domain.Payments.Entities;
using ShopApp.Domain.Payments.Enums;
using ShopApp.Domain.Payments.Repositories;

namespace ShopApp.Infrastructure.Persistence.Repositories;

public sealed class PaymentRepository(AppDbContext dbContext) : IPaymentRepository
{
    public Task<Payment?> GetByIdAsync(Guid id, CancellationToken ct = default) =>
        dbContext.Payments.FirstOrDefaultAsync(p => p.Id == id, ct);

    public async Task<(IReadOnlyList<Payment> Items, int TotalCount)> GetPagedAsync(
        Guid? userId, Guid? orderId, PaymentStatus? status,
        int page, int pageSize, CancellationToken ct = default)
    {
        var query = dbContext.Payments.AsQueryable();

        if (userId.HasValue)
            query = query.Where(p => p.UserId == userId.Value);

        if (orderId.HasValue)
            query = query.Where(p => p.OrderId == orderId.Value);

        if (status.HasValue)
            query = query.Where(p => p.Status == status.Value);

        query = query.OrderByDescending(p => p.CreatedAt);

        var totalCount = await query.CountAsync(ct);
        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

        return (items, totalCount);
    }

    public async Task AddAsync(Payment payment, CancellationToken ct = default) =>
        await dbContext.Payments.AddAsync(payment, ct);

    public void Update(Payment payment) =>
        dbContext.Payments.Update(payment);

    public void Delete(Payment payment) =>
        dbContext.Payments.Remove(payment);
}
