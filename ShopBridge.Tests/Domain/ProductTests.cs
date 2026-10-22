
using FluentAssertions;
using ShopBridge.Domain.Entities;

namespace ShopBridge.Tests.Domain;

public class ProductTests
{
    [Fact]
    public void Create_Product_Should_Set_Properties()
    {
        // Arrange
        string name = "Laptop";
        decimal price = 350000000;
        int stock = 3;

        // Act
        var product = new Product(name, price, stock);

        // Assert
        product.Id.Should().NotBeEmpty();
        product.Name.Should().Be("Laptop");
        product.Price.Should().Be(350000000);
        product.Stock.Should().Be(3);
    }
}
