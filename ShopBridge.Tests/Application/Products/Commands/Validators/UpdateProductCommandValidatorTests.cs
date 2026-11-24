using FluentValidation.TestHelper;
using ShopBridge.Application.Features.Products.Commands.UpdateProduct;
using ShopBridge.Application.Features.Products.Commands.Validators;

namespace ShopBridge.Tests.Application.Products.Commands.Validators;

public class UpdateProductCommandValidatorTests
{
    private readonly UpdateProductCommandValidator _validator = new();

    [Fact]
    public void Should_Have_Error_When_Name_Is_Empty()
    {
        var command = new UpdateProductCommand(Guid.NewGuid(), "", 1000, 5);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.name);
    }

    [Fact]
    public void Should_Have_Error_When_Price_Is_Not_Positive()
    {
        var command = new UpdateProductCommand(Guid.NewGuid(), "Laptop", 0, 5);

        var result = _validator.TestValidate(command);
        
        result.ShouldHaveValidationErrorFor(x => x.price);
    }

    [Fact]
    public void Should_Have_Error_When_Stock_Is_Negative()
    {
        var command = new UpdateProductCommand(Guid.NewGuid(), "Laptop", 1000, -1);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.stock);
    }
}
