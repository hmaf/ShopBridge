
using FluentAssertions;
using FluentValidation.TestHelper;
using ShopBridge.Application.Features.Products.Commands.CreateProduct;
using ShopBridge.Application.Features.Products.Commands.Validators;

namespace ShopBridge.Tests.Application.Products.Commands;

public class CreateProductCommandValidatorTests
{
    private readonly CreateProductCommandValidator _validator = new();

    [Fact]
    public void Should_Return_Error_When_Name_Is_Empty()
    {
        var command = new CreateProductCommand("", 100, 5);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Fact]
    public void Should_Return_Error_When_Price_Is_Zero()
    {
        var command = new CreateProductCommand("Laptop", 0, 5);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Price);
    }

    [Fact]
    public void Should_Return_Error_When_Stock_Is_Negative()
    {
        var command = new CreateProductCommand("Laptop", 100, -1);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Stock);
    }

    [Fact]
    public void Should_Not_Return_Error_For_Valid_Command()
    {
        var command = new CreateProductCommand("Laptop", 100, 5);

        var result = _validator.TestValidate(command);

        result.IsValid.Should().BeTrue();
    }
}
