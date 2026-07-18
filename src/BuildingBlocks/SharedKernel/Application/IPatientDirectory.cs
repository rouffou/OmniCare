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
}
