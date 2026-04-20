using FluentAssertions;
using Moq;
using ShopApp.Application.Common.Interfaces;
using ShopApp.Application.Orders.Commands.PlaceOrder;
using ShopApp.Domain.Catalog.Entities;
using ShopApp.Domain.Catalog.Repositories;
using ShopApp.Domain.Exceptions;
using ShopApp.Domain.Orders.Entities;
using ShopApp.Domain.Orders.Repositories;

namespace ShopApp.Tests.Application;

public sealed class PlaceOrderCommandHandlerTests
{
    private readonly Mock<IOrderRepository> _orderRepository = new();
    private readonly Mock<IProductRepository> _productRepository = new();
    private readonly Mock<IUnitOfWork> _unitOfWork = new();
    private readonly PlaceOrderCommandHandler _handler;

    public PlaceOrderCommandHandlerTests()
    {
        _handler = new PlaceOrderCommandHandler(
            _orderRepository.Object, _productRepository.Object, _unitOfWork.Object);
    }

    [Fact]
    public async Task Handle_ShouldPlaceOrder_WhenProductsExist()
    {
        var customerId = Guid.NewGuid();
        var productId = Guid.NewGuid();
        var product = Product.Create("Ebook C#", "Learn C#", 29.99m, "USD", "https://example.com/ebook.pdf");

        _productRepository.Setup(r => r.GetByIdAsync(productId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(product);
        _orderRepository.Setup(r => r.AddAsync(It.IsAny<Order>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        _unitOfWork.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var command = new PlaceOrderCommand(customerId, [new PlaceOrderItemDto(productId, 1)]);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Should().NotBeNull();
        result.CustomerId.Should().Be(customerId);
        result.Items.Should().ContainSingle();
        result.TotalAmount.Should().Be(29.99m);
    }

    [Fact]
    public async Task Handle_ShouldThrowDomainException_WhenProductNotFound()
    {
        var customerId = Guid.NewGuid();
        var missingProductId = Guid.NewGuid();

        _productRepository.Setup(r => r.GetByIdAsync(missingProductId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Product?)null);

        var command = new PlaceOrderCommand(customerId, [new PlaceOrderItemDto(missingProductId, 1)]);

        var act = () => _handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<DomainException>().WithMessage($"*{missingProductId}*");
    }
}
