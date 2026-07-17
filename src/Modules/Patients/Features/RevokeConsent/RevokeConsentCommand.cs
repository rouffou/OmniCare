using Mediarq.Core.Common.Requests.Command;
using Mediarq.Core.Common.Results;
using Mediarq.UnitOfWork;
using OmniCare.SharedKernel.Application.Auditing;

namespace OmniCare.Modules.Patients.Features.RevokeConsent;

/// <summary>
/// Révocation d'un consentement RGPD. L'historique est conservé (octroi + retrait
/// horodatés) — un nouvel octroi crée une nouvelle entrée. Commande auditée.
/// </summary>
public record RevokeConsentCommand(Guid PatientId, string ConsentType)
    : ICommand<Result<Guid>>, ITransactionalRequest, IAuditableRequest
{
    public string AuditAction => "Patient.RevokeConsent";
    public Guid? AuditTargetId => PatientId;
}
