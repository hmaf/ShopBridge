
using MediatR;
using ShopBridge.Application.Abstractions.Repositories;
using ShopBridge.Domain.Entities;

namespace ShopBridge.Application.Features.Products.Queries.GetProducts;

public class GetProductsQueryHandler(IProductRepository productRepo) : IRequestHandler<GetProductsQuery, IReadOnlyList<Product>>
{
    public async Task<IReadOnlyList<Product>> Handle(GetProductsQuery request, CancellationToken cancellationToken)
    {
        return await productRepo.GetAll(cancellationToken);
    }
}
