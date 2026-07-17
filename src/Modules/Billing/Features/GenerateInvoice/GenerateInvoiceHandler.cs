using Mediarq.Core.Common.Requests.Command;
using Mediarq.Core.Common.Results;
using OmniCare.Modules.Billing.Domain.Entities;
using OmniCare.Modules.Billing.Domain.ValueObjects;
using OmniCare.Modules.Billing.Infrastructure.Persistence;
using OmniCare.Modules.Billing.Infrastructure.Services;
using OmniCare.SharedKernel.Application;
using OmniCare.SharedKernel.Domain;
using OmniCare.SharedKernel.Domain.ValueObjects;

namespace OmniCare.Modules.Billing.Features.GenerateInvoice;

public class GenerateInvoiceHandler : ICommandHandler<GenerateInvoiceCommand, Result<Guid>>
{
    private readonly IBillingDbContext _context;
    private readonly IMyCareNetService _myCareNet;

    public GenerateInvoiceHandler(IBillingDbContext context, IMyCareNetService myCareNet)
    {
        _context = context;
        _myCareNet = myCareNet;
    }

    public async Task<Result<Guid>> Handle(GenerateInvoiceCommand request, CancellationToken cancellationToken = default)
    {
        // Validation métier via les Value Objects du Domaine (la validité du code pour
        // la profession du praticien relèvera du référentiel d'actes configurable).
        InamiCode code;
        try
        {
            code = InamiCode.Create(request.InamiCodeStr);
        }
        catch (DomainException ex)
        {
            return BusinessFailures.Rule<Guid>(ex.Message);
        }

        // Vérification d'assurabilité MyCareNet — obligatoire avant toute facturation (§4.3).
        var isInsured = await _myCareNet.VerifyAssurabilityAsync(request.PatientId, cancellationToken);

        var invoice = Invoice.CreateNew(
            request.PatientId,
            request.PractitionerId,
            code,
            Amount.Create(request.BaseAmount),
            isInsured);

        _context.Invoices.Add(invoice);
        // Commit par le UnitOfWorkBehavior (ITransactionalRequest) ; l'InvoiceGeneratedEvent
        // est publié après SaveChanges — à basculer sur Mediarq.Outbox avec la télétransmission.

        return Result.Success(invoice.Id);
    }
}
