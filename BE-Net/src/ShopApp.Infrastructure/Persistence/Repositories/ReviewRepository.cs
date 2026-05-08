using Microsoft.EntityFrameworkCore;
using ShopApp.Domain.Catalog.Entities;
using ShopApp.Domain.Catalog.Repositories;

namespace ShopApp.Infrastructure.Persistence.Repositories;

public sealed class ReviewRepository(AppDbContext dbContext) : IReviewRepository
{
    public Task<Review?> GetByIdAsync(Guid id, CancellationToken ct = default) =>
        dbContext.Reviews.FirstOrDefaultAsync(r => r.Id == id, ct);

    public async Task<IReadOnlyList<Review>> GetByProductIdAsync(Guid productId, CancellationToken ct = default) =>
        await dbContext.Reviews
            .Where(r => r.ProductId == productId)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync(ct);

    public Task<bool> ExistsByUserAndProductAsync(Guid userId, Guid productId, CancellationToken ct = default) =>
        dbContext.Reviews.AnyAsync(r => r.UserId == userId && r.ProductId == productId, ct);

    public async Task AddAsync(Review review, CancellationToken ct = default) =>
        await dbContext.Reviews.AddAsync(review, ct);

    public void Update(Review review) =>
        dbContext.Reviews.Update(review);

    public void Delete(Review review) =>
        dbContext.Reviews.Remove(review);
}
