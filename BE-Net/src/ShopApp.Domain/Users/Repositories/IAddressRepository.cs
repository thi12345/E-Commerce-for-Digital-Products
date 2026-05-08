using ShopApp.Domain.Users.Entities;

namespace ShopApp.Domain.Users.Repositories;

public interface IAddressRepository
{
    Task<Address?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<IReadOnlyList<Address>> GetByUserIdAsync(Guid userId, CancellationToken ct = default);
    Task AddAsync(Address address, CancellationToken ct = default);
    void Update(Address address);
    void Delete(Address address);
}
