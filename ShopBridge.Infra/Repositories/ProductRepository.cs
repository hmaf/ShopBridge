using Microsoft.EntityFrameworkCore;
using ShopBridge.Application.Abstractions.Repositories;
using ShopBridge.Domain.Entities;
using ShopBridge.Infra.Persistence;
using System.Runtime.CompilerServices;

namespace ShopBridge.Infra.Repositories;

public class ProductRepository : GenericRepository<Product>, IProductRepository
{
    public ProductRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<IReadOnlyList<Product>> GetAll(CancellationToken cancellationToken = default)
        => await _dbSet.AsNoTracking().ToListAsync(cancellationToken);
}
