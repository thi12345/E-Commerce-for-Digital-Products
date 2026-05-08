using Microsoft.EntityFrameworkCore;
using ShopApp.Domain.Catalog.Entities;
using ShopApp.Domain.Catalog.Enums;
using ShopApp.Domain.Catalog.Repositories;

namespace ShopApp.Infrastructure.Persistence.Repositories;

public sealed class ProductRepository(AppDbContext dbContext) : IProductRepository
{
    public Task<Product?> GetByIdAsync(Guid id, CancellationToken ct = default) =>
        ProductQuery().FirstOrDefaultAsync(p => p.Id == id, ct);

    public async Task<IReadOnlyList<Product>> GetAllAsync(CancellationToken ct = default) =>
        await ProductQuery().ToListAsync(ct);

    public async Task<IReadOnlyList<Product>> GetFilteredAsync(
        string? name, Guid? categoryId, decimal? minRating, decimal? minDiscountPercentage,
        ProductSortBy sortBy, CancellationToken ct = default)
    {
        var query = ProductQuery();

        if (!string.IsNullOrWhiteSpace(name))
            query = query.Where(p => p.Name.Value.Contains(name));

        if (categoryId.HasValue)
            query = query.Where(p => p.CategoryId == categoryId);

        if (minRating.HasValue)
            query = query.Where(p => p.RatingSummary.Average >= minRating.Value);

        if (minDiscountPercentage.HasValue)
            query = query.Where(p => p.Variants.Any(v => v.DiscountPercentage >= minDiscountPercentage.Value));

        query = ApplySort(query, sortBy);
        return await query.ToListAsync(ct);
    }

    public async Task<(IReadOnlyList<Product> Items, int TotalCount)> GetPagedAsync(
        string? name, Guid? categoryId, decimal? minRating, decimal? minDiscountPercentage,
        ProductSortBy sortBy, int page, int pageSize, CancellationToken ct = default)
    {
        var query = ProductQuery();

        if (!string.IsNullOrWhiteSpace(name))
            query = query.Where(p => p.Name.Value.Contains(name));

        if (categoryId.HasValue)
            query = query.Where(p => p.CategoryId == categoryId);

        if (minRating.HasValue)
            query = query.Where(p => p.RatingSummary.Average >= minRating.Value);

        if (minDiscountPercentage.HasValue)
            query = query.Where(p => p.Variants.Any(v => v.DiscountPercentage >= minDiscountPercentage.Value));

        query = ApplySort(query, sortBy);

        var totalCount = await query.CountAsync(ct);
        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

        return (items, totalCount);
    }

    private static IQueryable<Product> ApplySort(IQueryable<Product> query, ProductSortBy sortBy) =>
        sortBy switch
        {
            ProductSortBy.PriceAsc    => query.OrderBy(p => p.Variants.Min(v => v.DiscountedPrice.Amount)),
            ProductSortBy.PriceDesc   => query.OrderByDescending(p => p.Variants.Min(v => v.DiscountedPrice.Amount)),
            ProductSortBy.PurchaseAsc => query.OrderBy(p => p.PurchaseCount),
            ProductSortBy.PurchaseDesc => query.OrderByDescending(p => p.PurchaseCount),
            ProductSortBy.RatingAsc   => query.OrderBy(p => p.RatingSummary.Average),
            ProductSortBy.RatingDesc  => query.OrderByDescending(p => p.RatingSummary.Average),
            _                         => query
        };

    public async Task AddAsync(Product product, CancellationToken ct = default) =>
        await dbContext.Products.AddAsync(product, ct);

    public void Update(Product product) =>
        dbContext.Products.Update(product);

    public void Delete(Product product) =>
        dbContext.Products.Remove(product);

    private IQueryable<Product> ProductQuery() =>
        dbContext.Products
            .Include(p => p.Variants)
                .ThenInclude(v => v.Options);
}
