
using FluentAssertions;
using ShopBridge.Domain.Entities;
using ShopBridge.Domain.Exceptions;

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

    [Theory]
    [InlineData(0, 10)]
    [InlineData(-10, 10)]
    [InlineData(10, -10)]
    public void Create_WithInvalidPriceOrStock_ThrowsBusinessException(decimal price, int stock)
    {
        // Act
        var product = () => new Product("laptop", price, stock);

        // Assert
        Assert.Throws<BusinessException>(product);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Update_WithInvalidName_ThrowsBusinessException(string name)
    {
        var product = new Product("laptop", 1000, 2);

        var act = () => product.UpdateInformation(name, 1000, 2);

        act.Should().Throw<BusinessException>();
    }

    [Fact]
    public void Update_WithInvalidInformation_KeepsOriginalValues()
    {
        // Arrange
        var product = new Product("laptop", 1000, 2);

        // Act
        var act = () => product.UpdateInformation("", 2000, 5);

        // Assert
        act.Should().Throw<BusinessException>();

        product.Name.Should().Be("laptop");
        product.Price.Should().Be(1000);
        product.Stock.Should().Be(2);
    }
}
