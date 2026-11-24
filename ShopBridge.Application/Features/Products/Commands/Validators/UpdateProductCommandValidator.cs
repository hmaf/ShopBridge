
using FluentValidation;
using ShopBridge.Application.Features.Products.Commands.UpdateProduct;

namespace ShopBridge.Application.Features.Products.Commands.Validators;

public class UpdateProductCommandValidator
    : AbstractValidator<UpdateProductCommand>
{
    public UpdateProductCommandValidator()
    {
        RuleFor(o => o.id)
            .NotEmpty();

        RuleFor(o => o.name)
            .NotEmpty()
            .MinimumLength(2)
            .MaximumLength(200);

        RuleFor(o => o.price)
            .GreaterThan(0);

        RuleFor(o => o.stock)
            .GreaterThanOrEqualTo(0);
    }
}
