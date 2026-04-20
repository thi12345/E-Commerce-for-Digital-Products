using MediatR;
using ShopApp.Application.Orders.DTOs;

namespace ShopApp.Application.Orders.Commands.PlaceOrder;

public record PlaceOrderCommand(
    Guid CustomerId,
    IReadOnlyList<PlaceOrderItemDto> Items) : IRequest<OrderDto>;

public record PlaceOrderItemDto(Guid ProductId, int Quantity);
