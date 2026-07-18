using Mediarq.Core.Common.Requests.Command;
using Mediarq.Core.Common.Results;
using Mediarq.Outbox;
using Microsoft.EntityFrameworkCore;
using OmniCare.Modules.Billing.Domain.Events;
using OmniCare.Modules.Billing.Infrastructure.Persistence;
using OmniCare.SharedKernel.Application;
using OmniCare.SharedKernel.Domain;

namespace OmniCare.Modules.Billing.Features.RetryInvoiceTransmission;

public class RetryInvoiceTransmissionHandler : ICommandHandler<RetryInvoiceTransmissionCommand, Result<Guid>>
{
    private readonly IBillingDbContext _context;
    private readonly IOutbox _outbox;

    public RetryInvoiceTransmissionHandler(IBillingDbContext context, IOutbox outbox)
    {
        _context = context;
        _outbox = outbox;
    }

    public async Task<Result<Guid>> Handle(RetryInvoiceTransmissionCommand request, CancellationToken cancellationToken = default)
    {
        var invoice = await _context.Invoices
            .FirstOrDefaultAsync(i => i.Id == request.InvoiceId, cancellationToken);
        if (invoice is null)
            return BusinessFailures.NotFound<Guid>($"Facture {request.InvoiceId} introuvable.");

        try
        {
            invoice.RequestTransmissionRetry();
        }
        catch (DomainException ex)
        {
            return BusinessFailures.Rule<Guid>(ex.Message);
        }

        // Réutilise le même type d'événement : le handler de télétransmission ne distingue
        // pas un premier envoi d'une relance, il traite l'état courant de la facture.
        _outbox.Enqueue(new InvoiceGeneratedEvent(
            invoice.Id, invoice.PatientId, invoice.PractitionerId, invoice.Code.Value, invoice.Total.Value));

        return Result.Success(invoice.Id);
    }
}
