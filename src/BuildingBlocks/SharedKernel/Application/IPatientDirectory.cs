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
}
