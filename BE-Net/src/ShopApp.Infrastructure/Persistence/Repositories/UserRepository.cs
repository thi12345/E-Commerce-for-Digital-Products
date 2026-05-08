using Microsoft.EntityFrameworkCore;
using ShopApp.Domain.Users.Entities;
using ShopApp.Domain.Users.Enums;
using ShopApp.Domain.Users.Repositories;

namespace ShopApp.Infrastructure.Persistence.Repositories;

public sealed class UserRepository(AppDbContext dbContext) : IUserRepository
{
    public Task<User?> GetByIdAsync(Guid id, CancellationToken ct = default) =>
        dbContext.Users.FirstOrDefaultAsync(u => u.Id == id, ct);

    public Task<User?> GetByEmailAsync(string email, CancellationToken ct = default) =>
        dbContext.Users.FirstOrDefaultAsync(u => u.Email == email.Trim().ToLowerInvariant(), ct);

    public async Task<(IReadOnlyList<User> Items, int TotalCount)> GetPagedAsync(
        string? email, UserRole? role, bool? isActive,
        int page, int pageSize, CancellationToken ct = default)
    {
        var query = dbContext.Users.AsQueryable();

        if (!string.IsNullOrWhiteSpace(email))
            query = query.Where(u => u.Email.Contains(email.ToLowerInvariant()));

        if (role.HasValue)
            query = query.Where(u => u.Role == role.Value);

        if (isActive.HasValue)
            query = query.Where(u => u.IsActive == isActive.Value);

        query = query.OrderBy(u => u.CreatedAt);

        var totalCount = await query.CountAsync(ct);
        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

        return (items, totalCount);
    }

    public async Task AddAsync(User user, CancellationToken ct = default) =>
        await dbContext.Users.AddAsync(user, ct);

    public void Update(User user) =>
        dbContext.Users.Update(user);

    public void Delete(User user) =>
        dbContext.Users.Remove(user);
}
