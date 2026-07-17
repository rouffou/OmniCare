using OmniCare.Modules.Patients.Domain.Events;
using OmniCare.SharedKernel.Domain;
using OmniCare.SharedKernel.Domain.ValueObjects;

namespace OmniCare.Modules.Patients.Domain.Entities;

/// <summary>
/// Dossier médical du patient pour une profession de santé donnée — réservé au praticien
/// (séparation stricte avec les données administratives de <see cref="Patient"/>).
/// La structure (types d'entrées) est générique et profession-neutre : le contenu
/// d'un « bilan clinique » est paramétrable par profession, jamais codé en dur (§4.6).
/// Toute écriture doit passer par une commande auditée (audit trail RGPD/eHealth).
/// </summary>
public sealed class ClinicalRecord : AggregateRoot
{
    public Guid PatientId { get; private set; }
    public HealthProfession Profession { get; private set; }
    public DateTimeOffset OpenedOn { get; private set; }

    private readonly List<ClinicalEntry> _entries = [];
    public IReadOnlyCollection<ClinicalEntry> Entries => _entries.AsReadOnly();

    private readonly List<Prescription> _prescriptions = [];
    public IReadOnlyCollection<Prescription> Prescriptions => _prescriptions.AsReadOnly();

#pragma warning disable CS8618 // Constructeur de matérialisation EF Core
    private ClinicalRecord()
    {
    }
#pragma warning restore CS8618

    public static ClinicalRecord Open(Guid patientId, HealthProfession profession)
    {
        if (patientId == Guid.Empty)
            throw new DomainException("Un dossier médical doit être rattaché à un patient.");

        return new ClinicalRecord
        {
            PatientId = patientId,
            Profession = profession,
            OpenedOn = DateTimeOffset.UtcNow,
        };
    }

    public ClinicalEntry AddEntry(
        ClinicalEntryType type,
        string content,
        Guid authorPractitionerId,
        string? title = null)
    {
        if (string.IsNullOrWhiteSpace(content))
            throw new DomainException("Une entrée de dossier médical ne peut pas être vide.");
        if (authorPractitionerId == Guid.Empty)
            throw new DomainException("L'auteur (praticien) d'une entrée clinique est obligatoire.");

        var entry = ClinicalEntry.Create(Id, type, content.Trim(), authorPractitionerId, title);
        _entries.Add(entry);
        Raise(new ClinicalEntryAddedEvent(Id, PatientId, entry.Id, authorPractitionerId));
        return entry;
    }

    public Prescription RegisterPrescription(
        string prescriberName,
        DateOnly prescribedOn,
        int sessionsPrescribed)
    {
        var prescription = Prescription.Create(Id, prescriberName, prescribedOn, sessionsPrescribed);
        _prescriptions.Add(prescription);
        Raise(new PrescriptionRegisteredEvent(Id, PatientId, prescription.Id, sessionsPrescribed));
        return prescription;
    }

    /// <summary>
    /// Décompte une séance sur la prescription active la plus ancienne ayant un solde.
    /// Retourne <c>null</c> si aucune prescription n'a de séance restante — l'appelant
    /// (handler) traduit ce cas en échec métier Result, sans exception.
    /// </summary>
    public Prescription? ConsumeSessionFromOldestOpenPrescription()
    {
        var open = _prescriptions
            .Where(p => p.RemainingSessions > 0)
            .OrderBy(p => p.PrescribedOn)
            .FirstOrDefault();
        open?.ConsumeSession();
        return open;
    }
}
