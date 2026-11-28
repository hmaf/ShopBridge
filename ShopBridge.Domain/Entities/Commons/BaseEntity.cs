
namespace ShopBridge.Domain.Entities.Commons;

public abstract class BaseEntity<T>
{
    public T Id { get; protected set; } = default!;
    public DateTime CDate { get; private set; }
    public Guid? CreatedBy { get; private set; }
    public DateTime? UDate { get; private set; }
    public Guid? UpdatedBy { get; private set; }
    public DateTime? DDate { get; private set; }
    public Guid? DeletedBy { get; private set; }
    public bool IsDeleted => DDate.HasValue;

    public void SetCreated(DateTime date, Guid? userId)
    {
        CDate = date;
        CreatedBy = userId;
    }

    public void SetUpdated(DateTime date, Guid? userId)
    {
        UDate = date;
        UpdatedBy = userId;
    }

    public void SetDeleted(DateTime date, Guid? userId)
    {
        DDate = date;
        DeletedBy = userId;
    }
}
