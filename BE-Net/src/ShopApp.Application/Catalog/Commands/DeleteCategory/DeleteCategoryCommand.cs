using MediatR;

namespace ShopApp.Application.Catalog.Commands.DeleteCategory;

public record DeleteCategoryCommand(Guid Id) : IRequest;
