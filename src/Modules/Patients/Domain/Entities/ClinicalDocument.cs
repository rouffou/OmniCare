using OmniCare.SharedKernel.Domain;

namespace OmniCare.Modules.Patients.Domain.Entities;

/// <summary>
/// Document joint au dossier médical (prescription scannée, imagerie…) — cahier des
/// charges §4.1, ticket #31. Le contenu binaire n'est pas porté par cette entité : elle
/// ne référence qu'une <see cref="StorageKey"/> opaque résolue par <c>IDocumentStorage</c>
/// (chiffré au repos, indépendamment du fournisseur d'hébergement retenu).
/// </summary>
public sealed class ClinicalDocument : Entity
{
    public Guid ClinicalRecordId { get; private set; }
    public ClinicalDocumentType Type { get; private set; }
    public string FileName { get; private set; }
    public string ContentType { get; private set; }
    public long SizeBytes { get; private set; }
    public string StorageKey { get; private set; }
    public Guid UploadedByPractitionerId { get; private set; }
    public DateTimeOffset UploadedOn { get; private set; }

#pragma warning disable CS8618 // Constructeur de matérialisation EF Core
    private ClinicalDocument()
    {
    }
#pragma warning restore CS8618

    internal static ClinicalDocument Create(
        Guid clinicalRecordId,
        ClinicalDocumentType type,
        string fileName,
        string contentType,
        long sizeBytes,
        string storageKey,
        Guid uploadedByPractitionerId)
    {
        if (string.IsNullOrWhiteSpace(fileName))
            throw new DomainException("Le nom du fichier est obligatoire.");
        if (string.IsNullOrWhiteSpace(contentType))
            throw new DomainException("Le type de contenu du fichier est obligatoire.");
        if (sizeBytes <= 0)
            throw new DomainException("Un document joint ne peut pas être vide.");
        if (uploadedByPractitionerId == Guid.Empty)
            throw new DomainException("L'auteur (praticien) d'un document joint est obligatoire.");

        return new ClinicalDocument
        {
            ClinicalRecordId = clinicalRecordId,
            Type = type,
            FileName = fileName.Trim(),
            ContentType = contentType.Trim(),
            SizeBytes = sizeBytes,
            StorageKey = storageKey,
            UploadedByPractitionerId = uploadedByPractitionerId,
            UploadedOn = DateTimeOffset.UtcNow,
        };
    }
}

/// <summary>Types génériques, communs à toutes les professions de santé — pas de type
/// nommé par métier (§4.6).</summary>
public enum ClinicalDocumentType
{
    Prescription = 0,
    Imaging = 1,
    LabResult = 2,
    Other = 9,
}
