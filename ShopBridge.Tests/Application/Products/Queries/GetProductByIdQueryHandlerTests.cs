
using FluentAssertions;
using Moq;
using ShopBridge.Application.Abstractions.Repositories;
using ShopBridge.Application.Features.Products.Queries.GetProductById;
using ShopBridge.Domain.Entities;

namespace ShopBridge.Tests.Application.Products.Queries;

public class GetProductByIdQueryHandlerTests
{
    [Fact]
    public async Task Handle_Should_Return_Product_When_Product_Exists()
    {
        // Arrange
        var product = new Product("Laptop", 50000m, 10);
        var repositoryMock = new Mock<IGenericRepository<Product>>();

        repositoryMock
            .Setup(o => o.GetById(
                product.Id,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(product);

        var handler = new GetProductByIdQueryHandler(repositoryMock.Object);

        // Act 
        var result = await handler.Handle(
            new GetProductByIdQuery(product.Id),
            It.IsAny<CancellationToken>());

        // Assert
        result.Should().NotBeNull();
        result.Data!.Id.Should().Be(product.Id);
        result.Data.Name.Should().Be(product.Name);
        result.Data.Price.Should().Be(product.Price);
        result.Data.Stock.Should().Be(product.Stock);

        repositoryMock.Verify(
            o => o.GetById(product.Id, It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_Should_Return_Null_When_Product_Does_Not_Exist()
    {
        // arrange 
        var productId = Guid.NewGuid();
        var repositoryMock = new Mock<IGenericRepository<Product>>();

        repositoryMock.Setup(
            o => o.GetById(
                productId, 
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((Product?)null);

        var handler = new GetProductByIdQueryHandler(repositoryMock.Object);

        // Act
        var result = await handler.Handle(
            new GetProductByIdQuery(productId), 
            It.IsAny<CancellationToken>());

        // Assert 
        result.Data.Should().BeNull();
    }
}
