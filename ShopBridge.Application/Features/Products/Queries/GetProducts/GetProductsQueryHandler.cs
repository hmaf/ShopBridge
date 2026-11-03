
using Mapster;
using MediatR;
using ShopBridge.Application.Abstractions.Repositories;
using ShopBridge.Application.Common.Mapping;
using ShopBridge.Application.Features.Products.DTOs;
using ShopBridge.Domain.Entities;

namespace ShopBridge.Application.Features.Products.Queries.GetProducts;

public class GetProductsQueryHandler(IProductRepository productRepo)
    : IRequestHandler<GetProductsQuery, ApiResponse<List<ProductDto>>>
{
    public async Task<ApiResponse<List<ProductDto>>> Handle(GetProductsQuery request, CancellationToken cancellationToken)
    {
        var products = await productRepo.GetAll(cancellationToken);

        var result = products.Adapt<List<ProductDto>>();

        return ApiResponse<List<ProductDto>>.SuccessResponse(result);
    }
}
