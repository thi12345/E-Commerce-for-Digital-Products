using MediatR;
using ShopApp.Application.Catalog.DTOs;

namespace ShopApp.Application.Catalog.Commands.CreateCategory;

public record CreateCategoryCommand(string Name, string? Description) : IRequest<CategoryDto>;
