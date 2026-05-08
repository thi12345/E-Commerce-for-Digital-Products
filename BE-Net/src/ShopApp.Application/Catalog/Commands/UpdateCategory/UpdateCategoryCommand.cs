using MediatR;
using ShopApp.Application.Catalog.DTOs;

namespace ShopApp.Application.Catalog.Commands.UpdateCategory;

public record UpdateCategoryCommand(Guid Id, string Name, string? Description) : IRequest<CategoryDto>;
