
using Mapster;
using MediatR;
using Microsoft.IdentityModel.Tokens;
using ShopBridge.Application.Abstractions.Repositories;
using ShopBridge.Application.Common.Mapping;
using ShopBridge.Application.Features.Products.DTOs;
using ShopBridge.Domain.Entities;

namespace ShopBridge.Application.Features.Products.Queries.GetProductById;

public class GetProductByIdQueryHandler
    : IRequestHandler<GetProductByIdQuery, ApiResponse<ProductDto?>>
{
    private readonly IGenericRepository<Product> _repository;

    public GetProductByIdQueryHandler(IGenericRepository<Product> repository)
    {
        _repository = repository;
    }

    public async Task<ApiResponse<ProductDto?>> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
    {
        var product = await _repository.GetById(request.id, cancellationToken);

        if (product == null) return ApiResponse<ProductDto?>.FailureResponse("Product not found.");

        var result = product.Adapt<ProductDto>();

        return ApiResponse<ProductDto?>.SuccessResponse(result);
    }
}
