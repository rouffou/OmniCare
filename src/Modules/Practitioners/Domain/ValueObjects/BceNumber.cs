using System.Text.RegularExpressions;
using OmniCare.SharedKernel.Domain;

namespace OmniCare.Modules.Practitioners.Domain.ValueObjects;

/// <summary>
/// Numéro d'entreprise belge (BCE/KBO), 10 chiffres — obligatoire sur les documents
/// de facturation d'un cabinet. Seul le format est validé ici : l'algorithme de clé
/// de contrôle du BCE/KBO n'est pas implémenté faute de source officielle confirmée
/// à date de cette implémentation (contrairement au NISS, dont la clé mod 97 est
/// documentée publiquement — cf. <see cref="NationalRegistryNumber"/>). À compléter
/// si une vérification stricte devient nécessaire.
/// </summary>
public sealed partial record BceNumber
{
    public string Value { get; }

    private BceNumber(string value) => Value = value;

    public static BceNumber Create(string value)
    {
        var digits = Separators().Replace(value ?? string.Empty, "");
        if (!Format().IsMatch(digits))
            throw new DomainException(
                $"Numéro d'entreprise (BCE) invalide : « {value} ». Format attendu : 10 chiffres.");
        return new BceNumber(digits);
    }

    public string Formatted => $"BE{Value[..4]}.{Value[4..7]}.{Value[7..]}";

    public override string ToString() => Formatted;

    [GeneratedRegex(@"^\d{10}$")]
    private static partial Regex Format();

    [GeneratedRegex(@"[\s.\-]|^BE", RegexOptions.IgnoreCase)]
    private static partial Regex Separators();
}
