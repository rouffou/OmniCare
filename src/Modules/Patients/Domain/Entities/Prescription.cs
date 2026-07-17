using OmniCare.SharedKernel.Domain;

namespace OmniCare.Modules.Patients.Domain.Entities;

/// <summary>
/// Prescription médicale : nombre de séances prescrites et suivi de la consommation
/// (cahier des charges §4.1). Rattachée au dossier médical, pas à la fiche administrative.
/// </summary>
public sealed class Prescription : Entity
{
    public Guid ClinicalRecordId { get; private set; }
    public string PrescriberName { get; private set; }
    public DateOnly PrescribedOn { get; private set; }
    public int SessionsPrescribed { get; private set; }
    public int SessionsConsumed { get; private set; }

    public int RemainingSessions => SessionsPrescribed - SessionsConsumed;

#pragma warning disable CS8618 // Constructeur de matérialisation EF Core
    private Prescription()
    {
    }
#pragma warning restore CS8618

    internal static Prescription Create(
        Guid clinicalRecordId,
        string prescriberName,
        DateOnly prescribedOn,
        int sessionsPrescribed)
    {
        if (string.IsNullOrWhiteSpace(prescriberName))
            throw new DomainException("Le nom du prescripteur est obligatoire.");
        if (sessionsPrescribed <= 0)
            throw new DomainException("Une prescription doit porter sur au moins une séance.");

        return new Prescription
        {
            ClinicalRecordId = clinicalRecordId,
            PrescriberName = prescriberName.Trim(),
            PrescribedOn = prescribedOn,
            SessionsPrescribed = sessionsPrescribed,
        };
    }

    internal void ConsumeSession()
    {
        if (RemainingSessions <= 0)
            throw new DomainException("Toutes les séances de cette prescription ont déjà été consommées.");
        SessionsConsumed++;
    }
}
