namespace Wisgenix.SharedKernel.Domain;

/// <summary>
/// Base implementation for domain events
/// </summary>
public abstract class BaseDomainEvent : IDomainEvent
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}
