
using FluentValidation;
using ShopBridge.Application.Features.Products.Commands.CreateProduct;

namespace ShopBridge.Application.Features.Products.Commands.Validators;

public class CreateProductCommandValidator : 
    AbstractValidator<CreateProductCommand>
{
    public CreateProductCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MinimumLength(2)
            .MaximumLength(200);

        RuleFor(x => x.Price)
            .GreaterThan(0);

        RuleFor(x => x.Stock)
            .GreaterThanOrEqualTo(0);
    }
}
