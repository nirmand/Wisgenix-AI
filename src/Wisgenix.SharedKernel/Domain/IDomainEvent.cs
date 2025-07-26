using MediatR;

namespace Wisgenix.SharedKernel.Domain;

/// <summary>
/// Marker interface for domain events
/// </summary>
public interface IDomainEvent : INotification
{
    DateTime OccurredOn { get; }
}
