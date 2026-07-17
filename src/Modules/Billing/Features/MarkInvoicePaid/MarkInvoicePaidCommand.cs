using Mediarq.Core.Common.Requests.Command;
using Mediarq.Core.Common.Results;
using Mediarq.UnitOfWork;
using OmniCare.SharedKernel.Application.Auditing;

namespace OmniCare.Modules.Billing.Features.MarkInvoicePaid;

/// <summary>Encaissement d'une facture (suivi des paiements — cahier des charges §4.3). Commande auditée.</summary>
public record MarkInvoicePaidCommand(Guid InvoiceId, string PaymentMethod)
    : ICommand<Result<Guid>>, ITransactionalRequest, IAuditableRequest
{
    public string AuditAction => "Invoice.MarkPaid";
    public Guid? AuditTargetId => InvoiceId;
}
