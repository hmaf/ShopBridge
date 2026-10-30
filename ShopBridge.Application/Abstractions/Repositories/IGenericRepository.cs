
namespace ShopBridge.Application.Abstractions.Repositories;

public  interface IGenericRepository<T> where T : class
{
    Task<IReadOnlyList<T>> GetAll(
        CancellationToken cancellationToken = default);

    Task<T?> GetById(Guid id,
        CancellationToken cancellationToken = default);

    Task Add(T entity,
        CancellationToken cancellationToken = default);

    void Update(T entity);

    void Delete(T entity);
}
