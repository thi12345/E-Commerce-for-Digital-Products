using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using ShopApp.Application.Catalog.Commands.CreateProduct;
using ShopApp.Application.Common.Interfaces;
using ShopApp.Domain.Catalog.Entities;
using ShopApp.Domain.Catalog.Repositories;

namespace ShopApp.Tests.Application;

public sealed class CreateProductCommandHandlerTests
{
    private readonly Mock<IProductRepository> _productRepository = new();
    private readonly Mock<IUnitOfWork> _unitOfWork = new();
    private readonly CreateProductCommandHandler _handler;

    public CreateProductCommandHandlerTests()
    {
        _handler = new CreateProductCommandHandler(
            _productRepository.Object,
            _unitOfWork.Object,
            NullLogger<CreateProductCommandHandler>.Instance);
    }

    [Fact]
    public async Task Handle_ShouldCreateAndReturnProduct()
    {
        var command = new CreateProductCommand("Ebook C#", "Learn C#", 29.99m, "USD", "https://example.com/ebook.pdf");

        _productRepository.Setup(r => r.AddAsync(It.IsAny<Product>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        _unitOfWork.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Should().NotBeNull();
        result.Name.Should().Be("Ebook C#");
        result.Price.Should().Be(29.99m);
        result.Currency.Should().Be("USD");
        result.Id.Should().NotBeEmpty();
    }

    [Fact]
    public async Task Handle_ShouldCallAddAndSaveChanges()
    {
        var command = new CreateProductCommand("Ebook C#", "Learn C#", 29.99m, "USD", "https://example.com/ebook.pdf");

        _productRepository.Setup(r => r.AddAsync(It.IsAny<Product>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        _unitOfWork.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        await _handler.Handle(command, CancellationToken.None);

        _productRepository.Verify(r => r.AddAsync(It.IsAny<Product>(), It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldReturnDraftStatus()
    {
        var command = new CreateProductCommand("Ebook C#", "Learn C#", 29.99m, "USD", "https://example.com");

        _productRepository.Setup(r => r.AddAsync(It.IsAny<Product>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        _unitOfWork.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Status.Should().Be("Draft");
    }
}
