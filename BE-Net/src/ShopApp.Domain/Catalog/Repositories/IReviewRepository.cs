using ShopApp.Domain.Catalog.Entities;

namespace ShopApp.Domain.Catalog.Repositories;

public interface IReviewRepository
{
    Task<Review?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<IReadOnlyList<Review>> GetByProductIdAsync(Guid productId, CancellationToken ct = default);
    Task<bool> ExistsByUserAndProductAsync(Guid userId, Guid productId, CancellationToken ct = default);
    Task AddAsync(Review review, CancellationToken ct = default);
    void Update(Review review);
    void Delete(Review review);
}
