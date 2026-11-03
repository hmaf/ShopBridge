
using MediatR;
using ShopBridge.Application.Common.Mapping;
using ShopBridge.Application.Features.Products.DTOs;

namespace ShopBridge.Application.Features.Products.Queries.GetProductById;

public record GetProductByIdQuery(Guid id) 
    : IRequest<ApiResponse<ProductDto?>>
{
}
