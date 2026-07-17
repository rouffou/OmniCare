namespace OmniCare.Modules.Patients.Features.GetPatientById;

/// <summary>
/// Projection administrative du patient — aucune donnée clinique
/// (séparation secret médical, cahier des charges §4.1).
/// </summary>
public record PatientDto(
    Guid Id,
    string FirstName,
    string LastName,
    string? NationalRegistryNumberMasked,
    DateOnly? BirthDate,
    string? Email,
    string? Phone,
    string? AddressLine,
    string? PostalCode,
    string? City,
    string? MutualityCode,
    bool HasPreferentialRate,
    string InsurabilityState,
    DateOnly? InsurabilityCheckedOn,
    string? TreatingPhysicianName,
    string? EmergencyContact,
    Guid? ReferentPractitionerId,
    string Status,
    IReadOnlyList<ConsentDto> Consents);

public record ConsentDto(string Type, DateTimeOffset GrantedOn, DateTimeOffset? RevokedOn);
