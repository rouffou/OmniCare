using System.Text.RegularExpressions;

namespace OmniCare.SharedKernel.Domain.ValueObjects;

/// <summary>
/// Code de nomenclature INAMI : 6 chiffres. La validité du code pour une profession
/// donnée relève du référentiel d'actes configurable, pas de ce Value Object.
/// </summary>
public sealed partial record InamiCode : ActCode
{
    private InamiCode(string value) : base(value)
    {
    }

    public static InamiCode Create(string value)
    {
        var normalized = value?.Trim() ?? string.Empty;
        if (!Format().IsMatch(normalized))
            throw new InvalidActCodeException(
                $"Code INAMI invalide : « {value} ». Format attendu : 6 chiffres.");
        return new InamiCode(normalized);
    }

    [GeneratedRegex(@"^\d{6}$")]
    private static partial Regex Format();
}
