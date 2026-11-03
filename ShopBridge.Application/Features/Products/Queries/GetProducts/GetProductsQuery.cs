using MediatR;
using ShopBridge.Application.Common.Mapping;
using ShopBridge.Application.Features.Products.DTOs;
using ShopBridge.Domain.Entities;

namespace ShopBridge.Application.Features.Products.Queries.GetProducts;

public class GetProductsQuery : IRequest<ApiResponse<List<ProductDto>>>;
