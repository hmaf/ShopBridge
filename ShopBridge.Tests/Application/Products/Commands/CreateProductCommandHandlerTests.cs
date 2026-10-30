
using FluentAssertions;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using Moq;
using ShopBridge.Application.Abstractions.Persistence;
using ShopBridge.Application.Abstractions.Repositories;
using ShopBridge.Application.Features.Products.Commands.CreateProduct;
using ShopBridge.Domain.Entities;

namespace ShopBridge.Tests.Application.Products.Commands;

public class CreateProductCommandHandlerTests
{

    [Fact]
    public async Task Handle_Should_Create_Product_And_Save_Changes()
    {
        // Arrange
        var repository = new Mock<IGenericRepository<Product>>();
        var unitOfWork = new Mock<IUnitOfWork>();

        var command = new CreateProductCommand("PC", 1500, 5);

        var handler = new CreateProductCommandHandler(
            repository.Object,
            unitOfWork.Object);

        // Act
        var result = await handler.Handle(
            command,
            CancellationToken.None);

        // Assert
        result.Should().NotBeEmpty();

        repository.Verify(o =>
            o.Add(
                It.Is<Product>(p =>
                    p.Name == "PC"
                    && p.Price == 1500
                    && p.Stock == 5),
                It.IsAny<CancellationToken>()),
             Times.Once);

        unitOfWork.Verify(
            o => o.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Once);
    }
}
