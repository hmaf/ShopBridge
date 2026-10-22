
namespace ShopBridge.Domain.Entities.Commons;

public abstract class BaseEntity<T>
{
    public T Id { get; protected set; } = default!;
}
