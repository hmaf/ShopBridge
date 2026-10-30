using ShopBridge.Domain.Entities;

namespace ShopBridge.Application.Abstractions.Repositories;

public interface IProductRepository : IGenericRepository<Product>
{
    Task<IReadOnlyList<Product>> GetAll(CancellationToken cancellationToken = default(CancellationToken));
}
