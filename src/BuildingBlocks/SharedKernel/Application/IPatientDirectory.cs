namespace OmniCare.SharedKernel.Application;

/// <summary>
/// Contrat public minimal exposé par le module Patients aux autres modules
/// (Agenda, Billing) qui référencent un <c>PatientId</c> sans dépendre du
/// Domain/Infrastructure Patients — chaque module reste isolé (SQLite
/// multi-fichiers, pas de clé étrangère inter-modules). L'abstraction vit ici,
/// l'implémentation est fournie par le module Patients et branchée au
/// composition root (cf. CLAUDE.md, contrat inter-modules).
/// </summary>
public interface IPatientDirectory
{
    Task<bool> ExistsAsync(Guid patientId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Statut BIM/OMNIO du patient (régime préférentiel), <see langword="false"/> si le
    /// patient n'a pas de mutuelle affiliée ou n'en bénéficie pas. Ne détermine aucun
    /// montant de remboursement — le calcul du taux réel relève de la nomenclature INAMI
    /// (référentiel d'actes, hors périmètre de ce contrat) ; ce statut sert uniquement à
    /// tracer et informer la décision prise par le secrétariat au moment de facturer.
    /// </summary>
    Task<bool> HasPreferentialRateAsync(Guid patientId, CancellationToken cancellationToken = default);

    /// <summary>Identité utilisée pour les documents de facturation (cahier des charges
    /// §4.3) — l'adresse est <see langword="null"/> si le patient ne l'a pas renseignée
    /// (optionnelle côté fiche patient, cf. <c>ContactDetails</c>).</summary>
    Task<PatientIdentity?> GetIdentityAsync(Guid patientId, CancellationToken cancellationToken = default);
}

public sealed record PatientIdentity(
    Guid PatientId,
    string FullName,
    string? AddressLine,
    string? PostalCode,
    string? City);
