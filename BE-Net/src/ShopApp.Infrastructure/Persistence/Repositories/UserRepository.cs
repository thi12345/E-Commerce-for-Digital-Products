using Microsoft.EntityFrameworkCore;
using ShopApp.Domain.Users.Entities;
using ShopApp.Domain.Users.Repositories;

namespace ShopApp.Infrastructure.Persistence.Repositories;

public sealed class UserRepository(AppDbContext dbContext) : IUserRepository
{
    public async Task<IReadOnlyList<User>> GetAllAsync(CancellationToken ct = default) =>
        await dbContext.Users.AsNoTracking().OrderByDescending(u => u.CreatedAt).ToListAsync(ct);

    public async Task<User?> GetByIdAsync(Guid id, CancellationToken ct = default) =>
        await dbContext.Users.FindAsync([id], ct);

    public async Task AddAsync(User user, CancellationToken ct = default) =>
        await dbContext.Users.AddAsync(user, ct);

    public void Update(User user) => dbContext.Users.Update(user);

    public void Delete(User user) => dbContext.Users.Remove(user);
}
