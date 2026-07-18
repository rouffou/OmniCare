using Mediarq.Core.Common.Requests.Command;
using Mediarq.Core.Common.Results;
using Microsoft.EntityFrameworkCore;
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
    private readonly IPatientDirectory _patients;

    public GenerateInvoiceHandler(IBillingDbContext context, IMyCareNetService myCareNet, IPatientDirectory patients)
    {
        _context = context;
        _myCareNet = myCareNet;
        _patients = patients;
    }

    public async Task<Result<Guid>> Handle(GenerateInvoiceCommand request, CancellationToken cancellationToken = default)
    {
        if (!await _patients.ExistsAsync(request.PatientId, cancellationToken))
            return BusinessFailures.NotFound<Guid>($"Patient {request.PatientId} introuvable.");

        // Validation métier via les Value Objects du Domaine.
        InamiCode code;
        try
        {
            code = InamiCode.Create(request.InamiCodeStr);
        }
        catch (DomainException ex)
        {
            return BusinessFailures.Rule<Guid>(ex.Message);
        }

        var profession = HealthProfession.FromCode(request.ProfessionCode);

        // Le code doit exister au référentiel d'actes de la profession du praticien —
        // pas de logique de validation codée en dur par profession (cahier des charges §4.6).
        var catalogEntry = await _context.ActCatalogEntries
            .AsNoTracking()
            .FirstOrDefaultAsync(e => e.Profession == profession && e.Code == code, cancellationToken);
        if (catalogEntry is null)
            return BusinessFailures.NotFound<Guid>(
                $"Aucun acte {code.Value} au référentiel de la profession {profession.Code}.");
        if (!catalogEntry.IsActive)
            return BusinessFailures.Rule<Guid>(
                $"L'acte « {catalogEntry.Label} » ({code.Value}) est désactivé pour la profession {profession.Code}.");

        // Vérification d'assurabilité MyCareNet — obligatoire avant toute facturation (§4.3).
        var isInsured = await _myCareNet.VerifyAssurabilityAsync(request.PatientId, cancellationToken);

        // Statut BIM/OMNIO informatif (snapshot) — la part patient reste fournie par
        // l'appelant, cf. IPatientDirectory.HasPreferentialRateAsync.
        var hasPreferentialRate = await _patients.HasPreferentialRateAsync(request.PatientId, cancellationToken);

        Invoice invoice;
        try
        {
            invoice = Invoice.CreateNew(
                request.PatientId,
                request.PractitionerId,
                code,
                Amount.Create(request.BaseAmount),
                isInsured,
                hasPreferentialRate,
                Amount.Create(request.PatientShareAmount ?? request.BaseAmount),
                request.ThirdPartyPayer);
        }
        catch (DomainException ex)
        {
            return BusinessFailures.Rule<Guid>(ex.Message);
        }

        _context.Invoices.Add(invoice);
        // Commit par le UnitOfWorkBehavior (ITransactionalRequest) ; l'InvoiceGeneratedEvent
        // est publié après SaveChanges — à basculer sur Mediarq.Outbox avec la télétransmission.

        return Result.Success(invoice.Id);
    }
}
