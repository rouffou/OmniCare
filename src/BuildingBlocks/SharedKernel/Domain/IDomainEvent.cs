using Mediarq.Core.Common.Requests.Notifications;

namespace OmniCare.SharedKernel.Domain;

/// <summary>
/// Événement de domaine, publié via le médiateur Mediarq (INotification)
/// après persistance de l'agrégat qui l'a levé.
/// </summary>
public interface IDomainEvent : INotification
{
    DateTimeOffset OccurredOn { get; }
}
