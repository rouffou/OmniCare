using Mediarq.Core.Common.Requests.Notifications;
using Microsoft.EntityFrameworkCore;
using OmniCare.Modules.Billing.Domain.Events;
using OmniCare.Modules.Billing.Infrastructure.Persistence;
using OmniCare.Modules.Billing.Infrastructure.Services;

namespace OmniCare.Modules.Billing.Features.TransmitEAttest;

/// <summary>
/// Déclenche la télétransmission de l'eAttest lorsque l'événement, mis en file par
/// l'Outbox transactionnel (fiabilité — cf. GenerateInvoiceHandler), est publié par le
/// <c>OutboxProcessor</c> en arrière-plan. Persiste directement via le DbContext (hors
/// pipeline Mediarq Command/Query : ce n'est pas une requête utilisateur auditée, mais
/// un effet de bord asynchrone déclenché par un événement de domaine).
/// </summary>
public sealed class TransmitEAttestOnInvoiceGenerated : INotificationHandler<InvoiceGeneratedEvent>
{
    private readonly IBillingDbContext _context;
    private readonly IEHealthTransmissionService _transmission;

    public TransmitEAttestOnInvoiceGenerated(IBillingDbContext context, IEHealthTransmissionService transmission)
    {
        _context = context;
        _transmission = transmission;
    }

    public async Task Handle(InvoiceGeneratedEvent notification, CancellationToken cancellationToken = default)
    {
        var invoice = await _context.Invoices
            .FirstOrDefaultAsync(i => i.Id == notification.InvoiceId, cancellationToken);
        if (invoice is null)
            return;

        invoice.MarkTransmissionSent();
        await _context.SaveChangesAsync(cancellationToken);

        var result = await _transmission.SubmitEAttestAsync(invoice.Id, cancellationToken);
        if (result.Accepted)
            invoice.MarkTransmissionAccepted();
        else
            invoice.MarkTransmissionRejected(result.RejectionReason ?? "Rejet non précisé par l'organisme assureur.");

        await _context.SaveChangesAsync(cancellationToken);
    }
}
