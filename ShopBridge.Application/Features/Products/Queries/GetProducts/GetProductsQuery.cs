using MediatR;
using ShopBridge.Domain.Entities;

namespace ShopBridge.Application.Features.Products.Queries.GetProducts;

public class GetProductsQuery : IRequest<IReadOnlyList<Product>>;
