using OmniCare.SharedKernel.Domain;

namespace OmniCare.Modules.Practitioners.Domain.ValueObjects;

/// <summary>
/// Adresse d'un cabinet — contrairement à <c>ContactDetails</c> côté Patients (où
/// l'adresse est optionnelle), une adresse de cabinet est requise dès l'enregistrement :
/// elle figure obligatoirement sur les documents de facturation.
/// </summary>
public sealed record Address
{
    public string Line { get; private set; }
    public string PostalCode { get; private set; }
    public string City { get; private set; }

    private Address(string line, string postalCode, string city)
    {
        Line = line;
        PostalCode = postalCode;
        City = city;
    }

    public static Address Create(string line, string postalCode, string city)
    {
        if (string.IsNullOrWhiteSpace(line))
            throw new DomainException("L'adresse du cabinet est obligatoire.");
        if (string.IsNullOrWhiteSpace(postalCode))
            throw new DomainException("Le code postal du cabinet est obligatoire.");
        if (string.IsNullOrWhiteSpace(city))
            throw new DomainException("La localité du cabinet est obligatoire.");
        return new Address(line.Trim(), postalCode.Trim(), city.Trim());
    }

    public override string ToString() => $"{Line}, {PostalCode} {City}";
}
