using OmniCare.SharedKernel.Domain;

namespace OmniCare.Modules.Patients.Domain.ValueObjects;

public sealed record ContactDetails
{
    public string? Email { get; private set; }
    public string? Phone { get; private set; }
    public string? AddressLine { get; private set; }
    public string? PostalCode { get; private set; }
    public string? City { get; private set; }

    private ContactDetails(string? email, string? phone, string? addressLine, string? postalCode, string? city)
    {
        Email = email;
        Phone = phone;
        AddressLine = addressLine;
        PostalCode = postalCode;
        City = city;
    }

    public static ContactDetails Create(
        string? email = null,
        string? phone = null,
        string? addressLine = null,
        string? postalCode = null,
        string? city = null)
    {
        var normalizedEmail = Normalize(email);
        if (normalizedEmail is not null && !normalizedEmail.Contains('@'))
            throw new DomainException($"Adresse email invalide : « {email} ».");

        return new ContactDetails(
            normalizedEmail,
            Normalize(phone),
            Normalize(addressLine),
            Normalize(postalCode),
            Normalize(city));
    }

    public static readonly ContactDetails Empty = new(null, null, null, null, null);

    private static string? Normalize(string? value) =>
        string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}
