using MediatR;

namespace ShopApp.Application.Catalog.Commands.DeleteProduct;

public record DeleteProductCommand(Guid Id) : IRequest;
