using System.Text.RegularExpressions;

namespace OmniCare.SharedKernel.Domain.ValueObjects;

/// <summary>
/// Numéro de registre national belge (NISS) : 11 chiffres YYMMDD-XXX-CC,
/// où CC = 97 - (YYMMDDXXX mod 97), le nombre étant préfixé de « 2 »
/// pour les personnes nées à partir de 2000.
/// </summary>
public sealed partial record NationalRegistryNumber
{
    public string Value { get; }

    private NationalRegistryNumber(string value) => Value = value;

    public static NationalRegistryNumber Create(string value)
    {
        var digits = Separators().Replace(value ?? string.Empty, "");
        if (digits.Length != 11 || !digits.All(char.IsAsciiDigit))
            throw new InvalidNationalRegistryNumberException(
                $"Numéro de registre national invalide : « {value} ». 11 chiffres attendus.");

        var body = long.Parse(digits[..9]);
        var check = int.Parse(digits[9..]);

        var validBefore2000 = 97 - (int)(body % 97) == check;
        var validFrom2000 = 97 - (int)((2_000_000_000L + body) % 97) == check;
        if (!validBefore2000 && !validFrom2000)
            throw new InvalidNationalRegistryNumberException(
                $"Numéro de registre national invalide : « {value} ». Clé de contrôle incorrecte.");

        return new NationalRegistryNumber(digits);
    }

    /// <summary>Représentation masquée pour les logs (donnée à caractère personnel).</summary>
    public string Masked => $"{Value[..2]}*******{Value[^2..]}";

    public override string ToString() => Masked;

    [GeneratedRegex(@"[\s.\-]")]
    private static partial Regex Separators();
}

public sealed class InvalidNationalRegistryNumberException : DomainException
{
    public InvalidNationalRegistryNumberException(string message) : base(message)
    {
    }
}
