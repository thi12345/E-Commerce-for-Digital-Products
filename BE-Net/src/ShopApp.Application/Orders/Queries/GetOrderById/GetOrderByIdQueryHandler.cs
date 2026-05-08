using MediatR;
using Microsoft.Extensions.Logging;
using ShopApp.Application.Orders.DTOs;
using ShopApp.Domain.Orders.Repositories;

namespace ShopApp.Application.Orders.Queries.GetOrderById;

public sealed class GetOrderByIdQueryHandler(
    IOrderRepository orderRepository,
    ILogger<GetOrderByIdQueryHandler> logger)
    : IRequestHandler<GetOrderByIdQuery, OrderDto?>
{
    public async Task<OrderDto?> Handle(GetOrderByIdQuery request, CancellationToken ct)
    {
        logger.LogDebug("Fetching order: Id={OrderId}", request.Id);

        var o = await orderRepository.GetByIdAsync(request.Id, ct);

        if (o is null)
        {
            logger.LogWarning("Order not found: Id={OrderId}", request.Id);
            return null;
        }

        logger.LogDebug("Order found: Id={OrderId}, Status={Status}, Total={Total}",
            o.Id, o.Status, o.TotalAmount.Amount);

        return new OrderDto(
            o.Id, o.CustomerId, o.Status.ToString(),
            o.TotalAmount.Amount, o.TotalAmount.Currency,
            o.Items.Select(i => new OrderItemDto(
                i.ProductId, i.ProductName, i.UnitPrice.Amount, i.UnitPrice.Currency, i.Quantity, i.TotalPrice.Amount
            )).ToList().AsReadOnly(),
            o.CreatedAt);
    }
}
