using ShopBridge.Domain.Entities;

namespace ShopBridge.Application.Abstractions.Repositories;

public interface IProductRepository
{
    Task<IReadOnlyList<Product>> GetAll(CancellationToken cancellationToken = default(CancellationToken));
}
