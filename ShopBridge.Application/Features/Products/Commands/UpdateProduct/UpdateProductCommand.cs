using MediatR;
using ShopBridge.Application.Common.Mapping;

namespace ShopBridge.Application.Features.Products.Commands.UpdateProduct;

public record UpdateProductCommand(
    Guid id,
    string name,
    decimal price,
    int stock)
    : IRequest<ApiResponse<Guid>>;
 
