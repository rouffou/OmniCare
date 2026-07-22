using OmniCare.Modules.Patients.Domain.Events;
using OmniCare.Modules.Patients.Domain.ValueObjects;
using OmniCare.SharedKernel.Domain;
using OmniCare.SharedKernel.Domain.ValueObjects;

namespace OmniCare.Modules.Patients.Domain.Entities;

/// <summary>
/// Identité administrative du patient — accessible au secrétariat.
/// Les données cliniques vivent dans l'agrégat séparé <see cref="ClinicalRecord"/>
/// (séparation secret médical, cahier des charges §4.1).
/// </summary>
public sealed class Patient : AggregateRoot
{
    public PersonName Name { get; private set; }
    public NationalRegistryNumber? NationalRegistryNumber { get; private set; }
    public DateOnly? BirthDate { get; private set; }
    public ContactDetails Contact { get; private set; }
    public MutualityAffiliation? Mutuality { get; private set; }
    public InsurabilityStatus Insurability { get; private set; }
    public string? TreatingPhysicianName { get; private set; }
    public string? EmergencyContact { get; private set; }
    public Guid? ReferentPractitionerId { get; private set; }
    public PatientStatus Status { get; private set; }
    public DateTimeOffset RegisteredOn { get; private set; }

    /// <summary>Identifiant de l'utilisateur du portail patient (claim <c>sub</c> du token
    /// OIDC) — lié manuellement par le secrétariat après vérification d'identité (ticket
    /// #39), jamais déduit automatiquement d'un claim métier propre au fournisseur.</summary>
    public string? PortalUserId { get; private set; }

    private readonly List<Consent> _consents = [];
    public IReadOnlyCollection<Consent> Consents => _consents.AsReadOnly();

    private Patient(
        PersonName name,
        NationalRegistryNumber? nationalRegistryNumber,
        DateOnly? birthDate,
        ContactDetails contact)
    {
        Name = name;
        NationalRegistryNumber = nationalRegistryNumber;
        BirthDate = birthDate;
        Contact = contact;
        Insurability = InsurabilityStatus.Unknown;
        Status = PatientStatus.Active;
        RegisteredOn = DateTimeOffset.UtcNow;
    }

#pragma warning disable CS8618 // Constructeur de matérialisation EF Core
    private Patient()
    {
    }
#pragma warning restore CS8618

    public static Patient Register(
        PersonName name,
        NationalRegistryNumber? nationalRegistryNumber,
        DateOnly? birthDate,
        ContactDetails contact,
        MutualityAffiliation? mutuality = null,
        string? treatingPhysicianName = null,
        string? emergencyContact = null,
        Guid? referentPractitionerId = null)
    {
        var patient = new Patient(name, nationalRegistryNumber, birthDate, contact)
        {
            Mutuality = mutuality,
            TreatingPhysicianName = treatingPhysicianName,
            EmergencyContact = emergencyContact,
            ReferentPractitionerId = referentPractitionerId,
        };
        patient.Raise(new PatientRegisteredEvent(patient.Id));
        return patient;
    }

    public void UpdateContactDetails(ContactDetails contact)
    {
        EnsureActive();
        Contact = contact;
    }

    public void UpdateMutuality(MutualityAffiliation? mutuality)
    {
        EnsureActive();
        Mutuality = mutuality;
        // Un changement de mutuelle invalide la dernière vérification d'assurabilité.
        Insurability = InsurabilityStatus.Unknown;
    }

    public void RecordInsurabilityCheck(bool insured, DateOnly checkedOn)
    {
        EnsureActive();
        Insurability = InsurabilityStatus.Checked(insured, checkedOn);
    }

    public void GrantConsent(ConsentType type)
    {
        EnsureActive();
        var existing = _consents.FirstOrDefault(c => c.Type == type && c.IsActive);
        if (existing is not null)
            return;
        _consents.Add(Consent.Grant(Id, type));
    }

    public void RevokeConsent(ConsentType type)
    {
        EnsureActive();
        var active = _consents.FirstOrDefault(c => c.Type == type && c.IsActive)
            ?? throw new DomainException($"Aucun consentement actif de type {type} à révoquer.");
        active.Revoke();
    }

    public void LinkPortalAccount(string portalUserId)
    {
        EnsureActive();
        if (string.IsNullOrWhiteSpace(portalUserId))
            throw new DomainException("L'identifiant de compte portail ne peut pas être vide.");
        PortalUserId = portalUserId.Trim();
    }

    public void Archive()
    {
        if (Status == PatientStatus.Archived)
            return;
        Status = PatientStatus.Archived;
        Raise(new PatientArchivedEvent(Id));
    }

    private void EnsureActive()
    {
        if (Status != PatientStatus.Active)
            throw new DomainException("Opération impossible : la fiche patient est archivée.");
    }
}

public enum PatientStatus
{
    Active = 0,
    Archived = 1,
}
