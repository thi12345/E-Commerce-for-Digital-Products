using MediatR;
using Microsoft.Extensions.Logging;
using ShopApp.Application.Catalog.DTOs;
using ShopApp.Domain.Catalog.Repositories;

namespace ShopApp.Application.Catalog.Queries.GetCategoryById;

public sealed class GetCategoryByIdQueryHandler(
    ICategoryRepository categoryRepository,
    ILogger<GetCategoryByIdQueryHandler> logger)
    : IRequestHandler<GetCategoryByIdQuery, CategoryDto?>
{
    public async Task<CategoryDto?> Handle(GetCategoryByIdQuery request, CancellationToken ct)
    {
        var category = await categoryRepository.GetByIdAsync(request.Id, ct);

        if (category is null)
        {
            logger.LogWarning("Category not found: Id={CategoryId}", request.Id);
            return null;
        }

        return new CategoryDto(category.Id, category.Name, category.Description, category.CreatedAt);
    }
}
