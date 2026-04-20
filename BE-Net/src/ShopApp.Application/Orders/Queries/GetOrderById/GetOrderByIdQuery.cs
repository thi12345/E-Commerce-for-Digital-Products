using MediatR;
using ShopApp.Application.Orders.DTOs;

namespace ShopApp.Application.Orders.Queries.GetOrderById;

public record GetOrderByIdQuery(Guid Id) : IRequest<OrderDto?>;
