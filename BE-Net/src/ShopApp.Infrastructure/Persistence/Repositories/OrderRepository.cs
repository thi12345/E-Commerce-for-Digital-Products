using Microsoft.EntityFrameworkCore;
using ShopApp.Domain.Orders.Entities;
using ShopApp.Domain.Orders.Repositories;

namespace ShopApp.Infrastructure.Persistence.Repositories;

public sealed class OrderRepository(AppDbContext dbContext) : IOrderRepository
{
    public Task<Order?> GetByIdAsync(Guid id, CancellationToken ct = default) =>
        dbContext.Orders.Include(o => o.Items).FirstOrDefaultAsync(o => o.Id == id, ct);

    public async Task<IReadOnlyList<Order>> GetByCustomerIdAsync(Guid customerId, CancellationToken ct = default) =>
        await dbContext.Orders.Include(o => o.Items)
            .Where(o => o.CustomerId == customerId)
            .ToListAsync(ct);

    public async Task AddAsync(Order order, CancellationToken ct = default) =>
        await dbContext.Orders.AddAsync(order, ct);

    public void Update(Order order) =>
        dbContext.Orders.Update(order);
}
