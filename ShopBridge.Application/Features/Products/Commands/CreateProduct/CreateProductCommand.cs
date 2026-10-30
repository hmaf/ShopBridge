
using MediatR;

namespace ShopBridge.Application.Features.Products.Commands.CreateProduct;

public record CreateProductCommand(string Name,
        decimal Price,
        int Stock) 
    : IRequest<Guid>;

