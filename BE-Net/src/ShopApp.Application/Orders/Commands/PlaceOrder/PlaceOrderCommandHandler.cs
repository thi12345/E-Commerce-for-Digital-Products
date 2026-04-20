using MediatR;
using ShopApp.Application.Common.Interfaces;
using ShopApp.Application.Orders.DTOs;
using ShopApp.Domain.Catalog.Repositories;
using ShopApp.Domain.Exceptions;
using ShopApp.Domain.Orders.Entities;
using ShopApp.Domain.Orders.Repositories;

namespace ShopApp.Application.Orders.Commands.PlaceOrder;

public sealed class PlaceOrderCommandHandler(
    IOrderRepository orderRepository,
    IProductRepository productRepository,
    IUnitOfWork unitOfWork)
    : IRequestHandler<PlaceOrderCommand, OrderDto>
{
    public async Task<OrderDto> Handle(PlaceOrderCommand request, CancellationToken ct)
    {
        var order = Order.Create(request.CustomerId);

        foreach (var item in request.Items)
        {
            var product = await productRepository.GetByIdAsync(item.ProductId, ct)
                ?? throw new DomainException($"Product {item.ProductId} not found.");

            order.AddItem(product.Id, product.Name.Value, product.Price, item.Quantity);
        }

        order.Place();

        await orderRepository.AddAsync(order, ct);
        await unitOfWork.SaveChangesAsync(ct);

        return ToDto(order);
    }

    private static OrderDto ToDto(Order o) => new(
        o.Id, o.CustomerId, o.Status.ToString(),
        o.TotalAmount.Amount, o.TotalAmount.Currency,
        o.Items.Select(i => new OrderItemDto(
            i.ProductId, i.ProductName, i.UnitPrice.Amount, i.Quantity, i.TotalPrice.Amount
        )).ToList().AsReadOnly(),
        o.CreatedAt);
}
