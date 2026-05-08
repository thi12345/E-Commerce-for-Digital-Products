using Microsoft.EntityFrameworkCore;
using ShopApp.Domain.Users.Entities;
using ShopApp.Domain.Users.Repositories;

namespace ShopApp.Infrastructure.Persistence.Repositories;

public sealed class AddressRepository(AppDbContext dbContext) : IAddressRepository
{
    public Task<Address?> GetByIdAsync(Guid id, CancellationToken ct = default) =>
        dbContext.Addresses.FirstOrDefaultAsync(a => a.Id == id, ct);

    public async Task<IReadOnlyList<Address>> GetByUserIdAsync(Guid userId, CancellationToken ct = default) =>
        await dbContext.Addresses
            .Where(a => a.UserId == userId)
            .OrderByDescending(a => a.IsDefault)
            .ThenBy(a => a.CreatedAt)
            .ToListAsync(ct);

    public async Task AddAsync(Address address, CancellationToken ct = default) =>
        await dbContext.Addresses.AddAsync(address, ct);

    public void Update(Address address) =>
        dbContext.Addresses.Update(address);

    public void Delete(Address address) =>
        dbContext.Addresses.Remove(address);
}
