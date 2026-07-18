using Mediarq.Core.Common.Requests.Command;
using Mediarq.Core.Common.Results;
using Mediarq.Idempotency;
using Mediarq.UnitOfWork;
using OmniCare.SharedKernel.Application.Auditing;

namespace OmniCare.Modules.Billing.Features.GenerateInvoice;

/// <summary>
/// Génération d'une facture INAMI (cahier des charges §4.3). Commande auditée
/// (journalisation immuable de toute émission de facture), transactionnelle et
/// idempotente : la clé, fournie par l'appelant (en-tête Idempotency-Key ou
/// identifiant métier), protège contre les doubles émissions.
/// </summary>
public record GenerateInvoiceCommand(
    Guid PatientId,
    Guid PractitionerId,
    string ProfessionCode,
    string InamiCodeStr,
    decimal BaseAmount,
    string IdempotencyKey
) : ICommand<Result<Guid>>, ITransactionalRequest, IAuditableRequest, IIdempotentRequest
{
    public string AuditAction => "Invoice.Generate";
    public Guid? AuditTargetId => PatientId;
}
