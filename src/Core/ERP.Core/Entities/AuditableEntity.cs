namespace ERP.Core.Entities;

public class AuditableEntity<T>(T id) : Entity<T>(id), IAuditableEntity
{
    public DateTime CreatedAt { get; protected set; }
    public string CreatedBy { get; protected set; } = string.Empty;
    public DateTime? LastModifiedAt { get; protected set; }
    public string? LastModifiedBy { get; protected set; }
}