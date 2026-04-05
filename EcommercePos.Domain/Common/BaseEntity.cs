namespace EcommercePos.Domain.Common;

public abstract class BaseEntity<TId> where TId : struct
{
    public TId Id { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public Guid? CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public Guid? UpdatedBy { get; set; }
}

public abstract class AuditableEntity<TId> : BaseEntity<TId> where TId : struct
{
    public bool IsDeleted { get; set; } = false;
    public byte[]? RowVersion { get; set; }
}
