using System.Text.RegularExpressions;
using OmniCare.SharedKernel.Domain;

namespace OmniCare.Modules.Practitioners.Domain.ValueObjects;

/// <summary>
/// Numéro INAMI du praticien (identité du prestataire, 11 chiffres — distinct du
/// <c>InamiCode</c> de nomenclature qui identifie un acte, pas une personne). Seul
/// le format est validé, sans clé de contrôle (cf. remarque sur <see cref="BceNumber"/>
/// — même prudence, source officielle non confirmée à date).
/// </summary>
public sealed partial record PractitionerInamiNumber
{
    public string Value { get; }

    private PractitionerInamiNumber(string value) => Value = value;

    public static PractitionerInamiNumber Create(string value)
    {
        var digits = Separators().Replace(value ?? string.Empty, "");
        if (!Format().IsMatch(digits))
            throw new DomainException(
                $"Numéro INAMI de praticien invalide : « {value} ». Format attendu : 11 chiffres.");
        return new PractitionerInamiNumber(digits);
    }

    public override string ToString() => Value;

    [GeneratedRegex(@"^\d{11}$")]
    private static partial Regex Format();

    [GeneratedRegex(@"[\s.\-]")]
    private static partial Regex Separators();
}
