using Microsoft.EntityFrameworkCore;
using ShopBridge.Application.Abstractions.Repositories;
using ShopBridge.Domain.Entities;
using ShopBridge.Infra.Persistence;
using System.Runtime.CompilerServices;

namespace ShopBridge.Infra.Repositories;

public class ProductRepository(AppDbContext db) : IProductRepository
{
    
    public async Task<IReadOnlyList<Product>> GetAll(CancellationToken cancellationToken = default)
        => await db.Products.AsNoTracking().ToListAsync(cancellationToken);
}
