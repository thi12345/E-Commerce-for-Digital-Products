using MediatR;
using Microsoft.Extensions.Logging;
using ShopApp.Application.Catalog.DTOs;
using ShopApp.Application.Common.Interfaces;
using ShopApp.Domain.Catalog.Entities;
using ShopApp.Domain.Catalog.Repositories;
using ShopApp.Domain.Exceptions;

namespace ShopApp.Application.Catalog.Commands.CreateCategory;

public sealed class CreateCategoryCommandHandler(
    ICategoryRepository categoryRepository,
    IUnitOfWork unitOfWork,
    ILogger<CreateCategoryCommandHandler> logger)
    : IRequestHandler<CreateCategoryCommand, CategoryDto>
{
    public async Task<CategoryDto> Handle(CreateCategoryCommand request, CancellationToken ct)
    {
        if (await categoryRepository.ExistsByNameAsync(request.Name, null, ct))
            throw new DomainException($"Category with name '{request.Name}' already exists.");

        var category = Category.Create(request.Name, request.Description);

        await categoryRepository.AddAsync(category, ct);
        await unitOfWork.SaveChangesAsync(ct);

        logger.LogInformation("Category created: Id={CategoryId}, Name={Name}", category.Id, category.Name);

        return new CategoryDto(category.Id, category.Name, category.Description, category.CreatedAt);
    }
}
