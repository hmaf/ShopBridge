

using Microsoft.EntityFrameworkCore;
using ShopBridge.Application.Abstractions.Repositories;
using ShopBridge.Infra.Persistence;

namespace ShopBridge.Infra.Repositories;

public class GenericRepository<T> : IGenericRepository<T> where T : class
{
    protected readonly AppDbContext _context;
    protected readonly DbSet<T> _dbSet;

    public GenericRepository(AppDbContext context)
    {
        _context = context;
        _dbSet = context.Set<T>();
    }
    public async Task<IReadOnlyList<T>> GetAll(CancellationToken cancellationToken = default)
        => await _dbSet
                .AsNoTracking()
                .ToListAsync(cancellationToken);

    public async Task<T?> GetById(Guid id, CancellationToken cancellationToken = default)
        => await _dbSet.FindAsync(new object[] { id },cancellationToken);

    public async Task Add(T entity, CancellationToken cancellationToken = default)
        => await _dbSet.AddAsync(entity, cancellationToken);

    public void Update(T entity)
        => _dbSet.Update(entity);
    
    public void Delete(T entity)
        => _dbSet.Remove(entity);

}
