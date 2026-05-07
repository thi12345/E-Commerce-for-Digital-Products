using MediatR;
using ShopApp.Application.Orders.DTOs;
using ShopApp.Domain.Orders.Repositories;

namespace ShopApp.Application.Orders.Queries.GetOrderById;

public sealed class GetOrderByIdQueryHandler(IOrderRepository orderRepository)
    : IRequestHandler<GetOrderByIdQuery, OrderDto?>
{
    public async Task<OrderDto?> Handle(GetOrderByIdQuery request, CancellationToken ct)
    {
        var o = await orderRepository.GetByIdAsync(request.Id, ct);
        if (o is null) return null;

        return new OrderDto(
            o.Id, o.CustomerId, o.Status.ToString(),
            o.TotalAmount.Amount, o.TotalAmount.Currency,
            o.Items.Select(i => new OrderItemDto(
                i.ProductId, i.ProductName, i.UnitPrice.Amount, i.UnitPrice.Currency, i.Quantity, i.TotalPrice.Amount
            )).ToList().AsReadOnly(),
            o.CreatedAt);
    }
}
