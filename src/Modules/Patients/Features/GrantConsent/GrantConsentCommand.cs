using Mediarq.Core.Common.Requests.Command;
using Mediarq.Core.Common.Results;
using Mediarq.UnitOfWork;
using OmniCare.SharedKernel.Application.Auditing;

namespace OmniCare.Modules.Patients.Features.GrantConsent;

/// <summary>
/// Octroi d'un consentement RGPD (art. 9 — données de santé). Commande auditée :
/// la preuve de l'octroi doit être traçable.
/// </summary>
public record GrantConsentCommand(Guid PatientId, string ConsentType)
    : ICommand<Result<Guid>>, ITransactionalRequest, IAuditableRequest
{
    public string AuditAction => "Patient.GrantConsent";
    public Guid? AuditTargetId => PatientId;
}
