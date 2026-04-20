using Microsoft.EntityFrameworkCore;
using ShopApp.Domain.Catalog.Entities;
using ShopApp.Domain.Catalog.Repositories;

namespace ShopApp.Infrastructure.Persistence.Repositories;

public sealed class ProductRepository(AppDbContext dbContext) : IProductRepository
{
    public Task<Product?> GetByIdAsync(Guid id, CancellationToken ct = default) =>
        dbContext.Products.FirstOrDefaultAsync(p => p.Id == id, ct);

    public async Task<IReadOnlyList<Product>> GetAllAsync(CancellationToken ct = default) =>
        await dbContext.Products.ToListAsync(ct);

    public async Task AddAsync(Product product, CancellationToken ct = default) =>
        await dbContext.Products.AddAsync(product, ct);

    public void Update(Product product) =>
        dbContext.Products.Update(product);

    public void Delete(Product product) =>
        dbContext.Products.Remove(product);
}
