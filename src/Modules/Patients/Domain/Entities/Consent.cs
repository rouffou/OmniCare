using OmniCare.SharedKernel.Domain;

namespace OmniCare.Modules.Patients.Domain.Entities;

/// <summary>
/// Consentement RGPD du patient (art. 9 — données de santé).
/// La révocation ne supprime pas l'historique : on trace l'octroi et le retrait.
/// </summary>
public sealed class Consent : Entity
{
    public Guid PatientId { get; private set; }
    public ConsentType Type { get; private set; }
    public DateTimeOffset GrantedOn { get; private set; }
    public DateTimeOffset? RevokedOn { get; private set; }

    public bool IsActive => RevokedOn is null;

    private Consent()
    {
    }

    internal static Consent Grant(Guid patientId, ConsentType type) => new()
    {
        PatientId = patientId,
        Type = type,
        GrantedOn = DateTimeOffset.UtcNow,
    };

    internal void Revoke()
    {
        if (RevokedOn is not null)
            throw new DomainException("Ce consentement est déjà révoqué.");
        RevokedOn = DateTimeOffset.UtcNow;
    }
}

public enum ConsentType
{
    /// <summary>Traitement des données de santé par le cabinet.</summary>
    HealthDataProcessing = 0,

    /// <summary>Partage du dossier avec le médecin traitant / autres prestataires.</summary>
    SharingWithCareCircle = 1,

    /// <summary>Rappels et communications électroniques (SMS/email).</summary>
    ElectronicCommunication = 2,
}
