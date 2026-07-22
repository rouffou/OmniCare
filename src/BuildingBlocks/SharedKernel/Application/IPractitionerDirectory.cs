namespace OmniCare.SharedKernel.Application;

public interface IPractitionerDirectory
{
    Task<bool> ExistsAsync(Guid practitionerId, CancellationToken cancellationToken = default);

    /// <summary>Identité complète (praticien + cabinet) — utilisée pour les documents de
    /// facturation, qui doivent porter l'identification légale du prestataire et du
    /// cabinet (cahier des charges §4.3).</summary>
    Task<PractitionerIdentity?> GetIdentityAsync(Guid practitionerId, CancellationToken cancellationToken = default);
}

public sealed record PractitionerIdentity(
    Guid PractitionerId,
    string FullName,
    string ProfessionCode,
    string InamiNumber,
    Guid CabinetId,
    string CabinetName,
    string CabinetBceNumber,
    string CabinetAddressLine,
    string CabinetPostalCode,
    string CabinetCity);
