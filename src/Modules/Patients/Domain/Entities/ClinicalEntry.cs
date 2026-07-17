namespace OmniCare.Modules.Patients.Domain.Entities;

using OmniCare.SharedKernel.Domain;

/// <summary>
/// Entrée chronologique du dossier médical (anamnèse, bilan clinique, plan de traitement,
/// compte-rendu de séance…). Immuable après création : une correction se fait par ajout
/// d'une nouvelle entrée, jamais par réécriture de l'historique (exigence d'audit).
/// </summary>
public sealed class ClinicalEntry : Entity
{
    public Guid ClinicalRecordId { get; private set; }
    public ClinicalEntryType Type { get; private set; }
    public string? Title { get; private set; }
    public string Content { get; private set; }
    public Guid AuthorPractitionerId { get; private set; }
    public DateTimeOffset RecordedOn { get; private set; }

#pragma warning disable CS8618 // Constructeur de matérialisation EF Core
    private ClinicalEntry()
    {
    }
#pragma warning restore CS8618

    internal static ClinicalEntry Create(
        Guid clinicalRecordId,
        ClinicalEntryType type,
        string content,
        Guid authorPractitionerId,
        string? title) => new()
    {
        ClinicalRecordId = clinicalRecordId,
        Type = type,
        Content = content,
        AuthorPractitionerId = authorPractitionerId,
        Title = string.IsNullOrWhiteSpace(title) ? null : title.Trim(),
        RecordedOn = DateTimeOffset.UtcNow,
    };
}

/// <summary>
/// Types d'entrées génériques, communs à toutes les professions de santé.
/// Le contenu et les champs détaillés d'un « bilan clinique » sont paramétrables
/// par profession (référentiel de configuration, phase ultérieure) — pas de type
/// « bilan kiné » codé en dur.
/// </summary>
public enum ClinicalEntryType
{
    Anamnesis = 0,
    ClinicalAssessment = 1,
    TreatmentPlan = 2,
    SessionReport = 3,
    Other = 9,
}
