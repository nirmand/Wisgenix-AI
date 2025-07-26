namespace Wisgenix.SharedKernel.Domain;

/// <summary>
/// Base entity class that provides common properties for all domain entities
/// </summary>
public abstract class BaseEntity
{
    public int Id { get; protected set; }
    
    private readonly List<IDomainEvent> _domainEvents = new();
    
    public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();
    
    protected void AddDomainEvent(IDomainEvent domainEvent)
    {
        _domainEvents.Add(domainEvent);
    }
    
    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }
}
