using MediatR;
using Microsoft.Extensions.Logging;
using ShopApp.Application.Catalog.DTOs;
using ShopApp.Application.Common.Interfaces;
using ShopApp.Domain.Catalog.Repositories;
using ShopApp.Domain.Exceptions;

namespace ShopApp.Application.Catalog.Commands.UpdateCategory;

public sealed class UpdateCategoryCommandHandler(
    ICategoryRepository categoryRepository,
    IUnitOfWork unitOfWork,
    ILogger<UpdateCategoryCommandHandler> logger)
    : IRequestHandler<UpdateCategoryCommand, CategoryDto>
{
    public async Task<CategoryDto> Handle(UpdateCategoryCommand request, CancellationToken ct)
    {
        var category = await categoryRepository.GetByIdAsync(request.Id, ct)
            ?? throw new DomainException($"Category {request.Id} not found.");

        if (await categoryRepository.ExistsByNameAsync(request.Name, request.Id, ct))
            throw new DomainException($"Category with name '{request.Name}' already exists.");

        category.Update(request.Name, request.Description);
        categoryRepository.Update(category);
        await unitOfWork.SaveChangesAsync(ct);

        logger.LogInformation("Category updated: Id={CategoryId}, Name={Name}", category.Id, category.Name);

        return new CategoryDto(category.Id, category.Name, category.Description, category.CreatedAt);
    }
}
