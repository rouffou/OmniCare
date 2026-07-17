using Mediarq.Core.Common.Requests.Command;
using Mediarq.Core.Common.Results;
using Microsoft.EntityFrameworkCore;
using OmniCare.Modules.Billing.Domain.Entities;
using OmniCare.Modules.Billing.Infrastructure.Persistence;
using OmniCare.SharedKernel.Application;

namespace OmniCare.Modules.Billing.Features.MarkInvoicePaid;

public class MarkInvoicePaidHandler : ICommandHandler<MarkInvoicePaidCommand, Result<Guid>>
{
    private readonly IBillingDbContext _context;

    public MarkInvoicePaidHandler(IBillingDbContext context)
    {
        _context = context;
    }

    public async Task<Result<Guid>> Handle(MarkInvoicePaidCommand request, CancellationToken cancellationToken = default)
    {
        var invoice = await _context.Invoices
            .FirstOrDefaultAsync(i => i.Id == request.InvoiceId, cancellationToken);
        if (invoice is null)
            return BusinessFailures.NotFound<Guid>($"Facture {request.InvoiceId} introuvable.");

        if (invoice.Status != InvoiceStatus.Issued)
            return BusinessFailures.Rule<Guid>(
                $"Impossible d'encaisser une facture au statut {invoice.Status}.");

        invoice.MarkAsPaid(request.PaymentMethod);
        return Result.Success(invoice.Id);
    }
}
