using MediatR;
using Microsoft.Extensions.Logging;
using ShopApp.Application.Catalog.DTOs;
using ShopApp.Domain.Catalog.Repositories;

namespace ShopApp.Application.Catalog.Queries.GetCategories;

public sealed class GetCategoriesQueryHandler(
    ICategoryRepository categoryRepository,
    ILogger<GetCategoriesQueryHandler> logger)
    : IRequestHandler<GetCategoriesQuery, IReadOnlyList<CategoryDto>>
{
    public async Task<IReadOnlyList<CategoryDto>> Handle(GetCategoriesQuery request, CancellationToken ct)
    {
        var categories = await categoryRepository.GetAllAsync(ct);

        logger.LogInformation("Retrieved {Count} categories", categories.Count);

        return categories
            .Select(c => new CategoryDto(c.Id, c.Name, c.Description, c.CreatedAt))
            .ToList();
    }
}
