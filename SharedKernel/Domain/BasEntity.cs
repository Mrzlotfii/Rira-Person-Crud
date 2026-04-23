namespace SharedKernel.Domain;

public interface IBaseEntity
{
}

public abstract class BaseEntity<TId> : IBaseEntity
{

    protected BaseEntity()
    {
    }

    protected BaseEntity(TId id)
    {
        Id = id;
        CreatedAtUtc = DateTime.UtcNow;
        ModifiedAtUtc = DateTime.UtcNow;
    }

    protected BaseEntity(TId id, DateTime createdAtUtc, DateTime modifiedAtUtc)
    {
        Id = id;
        CreatedAtUtc = createdAtUtc;
        ModifiedAtUtc = modifiedAtUtc;
    }

    public TId Id { get; set; }
    public DateTime CreatedAtUtc { get; set; }
    public DateTime ModifiedAtUtc { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAtUtc { get; set; }
}
