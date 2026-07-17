using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace OmniCare.SharedKernel.Infrastructure;

/// <summary>
/// Stocke un DateTimeOffset en ticks UTC (INTEGER). Nécessaire pour toute colonne
/// comparée ou triée en SQL : le provider SQLite ne traduit pas les comparaisons
/// sur DateTimeOffset (TEXT). L'ordre chronologique est correct quel que soit
/// l'offset d'origine ; la valeur relue est exprimée en UTC.
/// </summary>
public sealed class UtcTicksConverter : ValueConverter<DateTimeOffset, long>
{
    public UtcTicksConverter()
        : base(v => v.UtcTicks, v => new DateTimeOffset(v, TimeSpan.Zero))
    {
    }
}
