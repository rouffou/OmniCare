using Mediarq.Core.Common.Requests.Command;
using Mediarq.Core.Common.Results;
using Mediarq.UnitOfWork;
using OmniCare.SharedKernel.Application.Auditing;

namespace OmniCare.Modules.Billing.Features.CancelInvoice;

/// <summary>
/// Annulation d'une facture émise non payée (correction d'erreur avant
/// télétransmission — cahier des charges §4.3). Commande auditée : la facture
/// annulée reste consultable, journalisation immuable.
/// </summary>
public record CancelInvoiceCommand(Guid InvoiceId, string? Reason)
    : ICommand<Result<Guid>>, ITransactionalRequest, IAuditableRequest
{
    public string AuditAction => "Invoice.Cancel";
    public Guid? AuditTargetId => InvoiceId;
}
