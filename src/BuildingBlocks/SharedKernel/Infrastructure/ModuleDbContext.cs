using Mediarq.Core.Mediators;
using Microsoft.EntityFrameworkCore;
using OmniCare.SharedKernel.Domain;

namespace OmniCare.SharedKernel.Infrastructure;

/// <summary>
/// Base des DbContext de modules : après un SaveChanges réussi, publie les Domain
/// Events collectés par les agrégats via Mediarq (publication in-memory — pour les
/// événements critiques ne tolérant aucune perte, basculer sur Mediarq.Outbox,
/// cf. architecture_technique_saas.md §4).
/// </summary>
public abstract class ModuleDbContext : DbContext
{
    private readonly IPublisher _publisher;

    protected ModuleDbContext(DbContextOptions options, IPublisher publisher)
        : base(options)
    {
        _publisher = publisher;
    }

    /// <summary>
    /// À appeler en fin de OnModelCreating : les Id sont générés côté client (Guid v7
    /// au constructeur), jamais par la base. Sans cela, EF considère qu'une entité
    /// enfant découverte via la navigation d'un agrégat tracké « existe déjà »
    /// (clé renseignée + génération OnAdd) et émet un UPDATE au lieu d'un INSERT.
    /// </summary>
    protected static void UseClientGeneratedIds(ModelBuilder modelBuilder)
    {
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            var id = entityType.FindProperty(nameof(Entity.Id));
            if (id is not null)
                id.ValueGenerated = Microsoft.EntityFrameworkCore.Metadata.ValueGenerated.Never;
        }
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var aggregates = ChangeTracker.Entries<AggregateRoot>()
            .Where(e => e.Entity.DomainEvents.Count > 0)
            .Select(e => e.Entity)
            .ToList();

        var written = await base.SaveChangesAsync(cancellationToken);

        foreach (var aggregate in aggregates)
        {
            var events = aggregate.DomainEvents.ToList();
            aggregate.ClearDomainEvents();
            foreach (var domainEvent in events)
                await _publisher.Publish(domainEvent, cancellationToken);
        }

        return written;
    }
}
