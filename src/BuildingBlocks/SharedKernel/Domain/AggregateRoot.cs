using System.ComponentModel.DataAnnotations.Schema;

namespace OmniCare.SharedKernel.Domain;

/// <summary>
/// Racine d'agrégat : seule porte d'entrée des modifications de l'agrégat,
/// collecte les Domain Events levés par les méthodes métier.
/// </summary>
public abstract class AggregateRoot : Entity
{
    private readonly List<IDomainEvent> _domainEvents = [];

    [NotMapped]
    public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    protected void Raise(IDomainEvent domainEvent) => _domainEvents.Add(domainEvent);

    public void ClearDomainEvents() => _domainEvents.Clear();
}
