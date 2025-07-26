namespace Wisgenix.SharedKernel.Domain;

/// <summary>
/// Base auditable entity that includes audit fields
/// </summary>
public abstract class AuditableEntity : BaseEntity
{
    public DateTime CreatedDate { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime? ModifiedDate { get; set; }
    public string? ModifiedBy { get; set; }
}
