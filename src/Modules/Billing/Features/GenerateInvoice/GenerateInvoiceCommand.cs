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
// PatientShareAmount : part à charge du patient. Si omise, le patient est facturé du
// montant total (pas de tiers payant).
// ThirdPartyPayer : la part mutuelle (BaseAmount - PatientShareAmount) est réclamée
// directement à l'organisme assureur. Nécessite PatientShareAmount < BaseAmount.
public record GenerateInvoiceCommand(
    Guid PatientId,
    Guid PractitionerId,
    string ProfessionCode,
    string InamiCodeStr,
    decimal BaseAmount,
    string IdempotencyKey,
    decimal? PatientShareAmount = null,
    bool ThirdPartyPayer = false
) : ICommand<Result<Guid>>, ITransactionalRequest, IAuditableRequest, IIdempotentRequest
{
    public string AuditAction => "Invoice.Generate";
    public Guid? AuditTargetId => PatientId;
}
