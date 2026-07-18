using Mediarq.Core.Common.Requests.Command;
using Mediarq.Core.Common.Results;
using Mediarq.UnitOfWork;
using OmniCare.SharedKernel.Application.Auditing;

namespace OmniCare.Modules.Billing.Features.RetryInvoiceTransmission;

/// <summary>
/// Relance la télétransmission d'une facture rejetée, après correction (cahier des
/// charges §4.3, gestion des rejets/corrections). Auditée : action sur une facture.
/// </summary>
public record RetryInvoiceTransmissionCommand(Guid InvoiceId)
    : ICommand<Result<Guid>>, ITransactionalRequest, IAuditableRequest
{
    public string AuditAction => "Invoice.RetryTransmission";
    public Guid? AuditTargetId => InvoiceId;
}
