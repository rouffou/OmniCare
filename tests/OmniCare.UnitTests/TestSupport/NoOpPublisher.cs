using Mediarq.Core.Common.Requests.Notifications;
using Mediarq.Core.Mediators;

namespace OmniCare.UnitTests.TestSupport;

/// <summary>Publisher factice pour instancier un ModuleDbContext en test sans DI complète —
/// les Domain Events levés ne sont pas consommés, seul le comportement du Handler testé
/// compte ici (vérifications d'appartenance du portail patient, ticket #39).</summary>
public sealed class NoOpPublisher : IPublisher
{
    public Task Publish<TNotification>(TNotification notification, CancellationToken cancellationToken = default)
        where TNotification : INotification
        => Task.CompletedTask;
}
