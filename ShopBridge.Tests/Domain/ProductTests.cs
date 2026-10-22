
using FluentAssertions;
using ShopBridge.Domain.Entities;

namespace ShopBridge.Tests.Domain;

public class ProductTests
{
    [Fact]
    public void Create_Product_Should_Set_Properties()
    {
        // Arrange 
        var product = new Product(
            "Laptop",
            350000000,
            3);

        // Act
        var result = product;

        // Assert
        result.Id.Should().NotBeEmpty();
        result.Name.Should().Be("Laptop");
        result.Price.Should().Be(350000000);
        result.Stock.Should().Be(3);
    }
}
