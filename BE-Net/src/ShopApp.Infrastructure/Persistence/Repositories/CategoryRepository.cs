using Microsoft.EntityFrameworkCore;
using ShopApp.Domain.Catalog.Entities;
using ShopApp.Domain.Catalog.Repositories;

namespace ShopApp.Infrastructure.Persistence.Repositories;

public sealed class CategoryRepository(AppDbContext dbContext) : ICategoryRepository
{
    public Task<Category?> GetByIdAsync(Guid id, CancellationToken ct = default) =>
        dbContext.Categories.FirstOrDefaultAsync(c => c.Id == id, ct);

    public async Task<IReadOnlyList<Category>> GetAllAsync(CancellationToken ct = default) =>
        await dbContext.Categories.OrderBy(c => c.Name).ToListAsync(ct);

    public Task<bool> ExistsByNameAsync(string name, Guid? excludeId = null, CancellationToken ct = default) =>
        dbContext.Categories.AnyAsync(c => c.Name == name && c.Id != excludeId, ct);

    public async Task AddAsync(Category category, CancellationToken ct = default) =>
        await dbContext.Categories.AddAsync(category, ct);

    public void Update(Category category) =>
        dbContext.Categories.Update(category);

    public void Delete(Category category) =>
        dbContext.Categories.Remove(category);
}
