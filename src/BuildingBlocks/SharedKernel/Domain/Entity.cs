namespace OmniCare.SharedKernel.Domain;

/// <summary>
/// Entité DDD : identité stable (Guid v7, ordonné pour l'indexation),
/// égalité par identifiant.
/// </summary>
public abstract class Entity
{
    public Guid Id { get; protected set; } = Guid.CreateVersion7();

    public override bool Equals(object? obj) =>
        obj is Entity other && GetType() == other.GetType() && Id == other.Id;

    public override int GetHashCode() => HashCode.Combine(GetType(), Id);
}
