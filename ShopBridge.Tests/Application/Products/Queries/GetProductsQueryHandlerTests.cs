

using FluentAssertions;
using MediatR.NotificationPublishers;
using Moq;
using ShopBridge.Application.Abstractions.Repositories;
using ShopBridge.Application.Features.Products.Queries.GetProducts;
using ShopBridge.Domain.Entities;
using FluentAssertions;

namespace ShopBridge.Tests.Application.Products.Queries;

public class GetProductsQueryHandlerTests
{
    [Fact]
    public async Task Handle_Should_Return_Products_From_Repository()
    {
        // Arrange
        var products = new List<Product>
        {
            new("Laptop", 50000m, 10),
            new("Mouse", 1000m, 20)
        };

        var repositoryMock = new Mock<IProductRepository>();

        repositoryMock
            .Setup(o => o.GetAll(It.IsAny<CancellationToken>()))
            .ReturnsAsync(products);

        var handler = new GetProductsQueryHandler(repositoryMock.Object);

        // Act
        var result = await handler.Handle(
            new GetProductsQuery(),
            CancellationToken.None);

        // Assert
        result.Should().HaveCount(2);
        result.Should().BeEquivalentTo(products);

        repositoryMock.Verify(
            o => o.GetAll(It.IsAny<CancellationToken>()),
            Times.Once());
    }

    [Fact]
    public async Task Handle_Should_Return_Empty_List_When_No_Products_Exist()
    {
        // Arrange
        var repositoryMock = new Mock<IProductRepository>();

        repositoryMock
            .Setup(o => o.GetAll(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Product>());

        var handler = new GetProductsQueryHandler(repositoryMock.Object);

        // Act
        var result = await handler.Handle(new GetProductsQuery(), CancellationToken.None);

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public async Task Handle_Should_Pass_CancellationToken_To_Repository()
    {
        // Arrange
        using var cts = new CancellationTokenSource();
        var expectedToken = cts.Token;

        var repositoryMock = new Mock<IProductRepository>();

        repositoryMock
            .Setup(x => x.GetAll(expectedToken))
            .ReturnsAsync(new List<Product>());

        var handler = new GetProductsQueryHandler(repositoryMock.Object);

        // Act
        await handler.Handle(new GetProductsQuery(), expectedToken);

        // Assert
        repositoryMock.Verify(
            x => x.GetAll(expectedToken),
            Times.Once);
    }
}
