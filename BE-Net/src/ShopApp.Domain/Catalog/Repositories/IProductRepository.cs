using ShopApp.Domain.Catalog.Entities;
using ShopApp.Domain.Catalog.Enums;

namespace ShopApp.Domain.Catalog.Repositories;

public interface IProductRepository
{
    Task<Product?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<IReadOnlyList<Product>> GetAllAsync(CancellationToken ct = default);
    Task<IReadOnlyList<Product>> GetFilteredAsync(
        string? name,
        Guid? categoryId,
        decimal? minRating,
        decimal? minDiscountPercentage,
        ProductSortBy sortBy,
        CancellationToken ct = default);

    Task<(IReadOnlyList<Product> Items, int TotalCount)> GetPagedAsync(
        string? name,
        Guid? categoryId,
        decimal? minRating,
        decimal? minDiscountPercentage,
        ProductSortBy sortBy,
        int page,
        int pageSize,
        CancellationToken ct = default);
    Task AddAsync(Product product, CancellationToken ct = default);
    void Update(Product product);
    void Delete(Product product);
}
