using Mediarq.Core.Common.Requests.Command;
using Mediarq.Core.Common.Results;
using Mediarq.UnitOfWork;
using OmniCare.SharedKernel.Application.Auditing;

namespace OmniCare.Modules.Patients.Features.LinkPatientPortalAccount;

/// <summary>
/// Lie le compte OIDC du portail patient (ticket #39) à une fiche patient existante.
/// Le secrétariat vérifie l'identité du patient (en cabinet ou par un canal sûr) avant
/// de lier — aucune vérification automatique n'est faite ici, OmniCare ne pilote pas
/// le fournisseur d'identité. Commande auditée : la preuve de la liaison doit être
/// traçable (accès aux données de santé du patient depuis son propre compte).
/// </summary>
public record LinkPatientPortalAccountCommand(Guid PatientId, string PortalUserId)
    : ICommand<Result<Guid>>, ITransactionalRequest, IAuditableRequest
{
    public string AuditAction => "Patient.LinkPortalAccount";
    public Guid? AuditTargetId => PatientId;
}
