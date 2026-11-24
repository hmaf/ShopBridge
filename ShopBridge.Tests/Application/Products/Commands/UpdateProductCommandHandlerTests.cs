using FluentAssertions;
using Moq;
using ShopBridge.Application.Abstractions.Persistence;
using ShopBridge.Application.Abstractions.Repositories;
using ShopBridge.Application.Features.Products.Commands.UpdateProduct;
using ShopBridge.Domain.Entities;
using ShopBridge.Domain.Exceptions;
using System.Runtime.CompilerServices;

namespace ShopBridge.Tests.Application.Products.Commands;

public class UpdateProductCommandHandlerTests
{
    [Fact]
    public async Task Handle_Should_Update_Product_And_Save_Changes()
    {
        // Arrange
        var product = new Product("Laptop", 1000, 5);

        var repo = new Mock<IGenericRepository<Product>>();
        var uow = new Mock<IUnitOfWork>();

        var command = new UpdateProductCommand(
            product.Id,
            "Gaming Laptop",
            2000,
            10);

        repo.Setup(o => o.GetById(command.id,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(product);
                

        var handler = new UpdateProductCommandHandler(
            repo.Object, 
            uow.Object);

        // Act
        var result = await handler.Handle(
            command,
            CancellationToken.None);

        // Assert
        result.Data.Should().Be(product.Id);

        product.Name.Should().Be("Gaming Laptop");
        product.Price.Should().Be(2000);
        product.Stock.Should().Be(10);

        uow.Verify(
            o => o.SaveChangesAsync(It.IsAny<CancellationToken>()), 
            Times.Once());
    }

    [Fact]
    public async Task Handle_Should_Throw_BusinessException_When_Product_NotFound()
    {
        // Arrange
        var repo = new Mock<IGenericRepository<Product>>();
        var uow = new Mock<IUnitOfWork>();

        var command = new UpdateProductCommand(
            Guid.NewGuid(),
            "Laptop",
            1000,
            10);

        repo.Setup(o => o.GetById(
                command.id,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((Product?)null);

        var handler = new UpdateProductCommandHandler(
            repo.Object,
            uow.Object);

        // Act
        var act = () => handler.Handle(
            command,
            CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<BusinessException>();

        uow.Verify(o => 
            o.SaveChangesAsync(It.IsAny<CancellationToken>()), 
            Times.Never);
    }
}
