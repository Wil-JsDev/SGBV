namespace SGBV.Domain.Common;

public abstract class BaseEntity
{
    public Guid Id { get; set; }
    public DateTime CreatedOnUtc { get; set; }

    public DateTime? UpdatedOnUtc { get; set; }

    public bool IsDeleted { get; set; } = false;

    public DateTime? DeletedOnUtc { get; set; }
}