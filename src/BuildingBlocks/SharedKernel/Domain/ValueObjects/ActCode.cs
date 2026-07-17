namespace OmniCare.SharedKernel.Domain.ValueObjects;

/// <summary>
/// Code d'acte générique. Chaque système de nomenclature (INAMI en Belgique)
/// en dérive avec ses propres règles de format — le Domain ne connaît que ce type,
/// le référentiel des codes valides par profession est configurable (cf. cahier des
/// charges §4.6, extensibilité multi-professions).
/// </summary>
public abstract record ActCode
{
    public string Value { get; }

    protected ActCode(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new InvalidActCodeException("Un code d'acte ne peut pas être vide.");
        Value = value;
    }

    public sealed override string ToString() => Value;
}

public sealed class InvalidActCodeException : DomainException
{
    public InvalidActCodeException(string message) : base(message)
    {
    }
}
