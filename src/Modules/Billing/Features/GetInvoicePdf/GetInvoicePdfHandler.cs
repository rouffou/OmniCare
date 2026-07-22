using Mediarq.Core.Common.Requests.Query;
using Mediarq.Core.Common.Results;
using Microsoft.EntityFrameworkCore;
using OmniCare.Modules.Billing.Infrastructure.Persistence;
using OmniCare.SharedKernel.Application;
using OmniCare.SharedKernel.Domain.ValueObjects;

namespace OmniCare.Modules.Billing.Features.GetInvoicePdf;

public class GetInvoicePdfHandler : IQueryHandler<GetInvoicePdfQuery, Result<byte[]>>
{
    private readonly IBillingDbContext _context;
    private readonly IPatientDirectory _patients;
    private readonly IPractitionerDirectory _practitioners;

    public GetInvoicePdfHandler(
        IBillingDbContext context, IPatientDirectory patients, IPractitionerDirectory practitioners)
    {
        _context = context;
        _patients = patients;
        _practitioners = practitioners;
    }

    public async Task<Result<byte[]>> Handle(GetInvoicePdfQuery request, CancellationToken cancellationToken = default)
    {
        var invoice = await _context.Invoices
            .AsNoTracking()
            .FirstOrDefaultAsync(i => i.Id == request.InvoiceId, cancellationToken);
        if (invoice is null)
            return BusinessFailures.NotFound<byte[]>($"Facture {request.InvoiceId} introuvable.");

        var practitioner = await _practitioners.GetIdentityAsync(invoice.PractitionerId, cancellationToken);
        if (practitioner is null)
            return BusinessFailures.NotFound<byte[]>($"Praticien {invoice.PractitionerId} introuvable.");

        var patient = await _patients.GetIdentityAsync(invoice.PatientId, cancellationToken);
        if (patient is null)
            return BusinessFailures.NotFound<byte[]>($"Patient {invoice.PatientId} introuvable.");

        var profession = HealthProfession.FromCode(practitioner.ProfessionCode);
        var catalogEntry = await _context.ActCatalogEntries
            .AsNoTracking()
            .FirstOrDefaultAsync(e => e.Profession == profession && e.Code == invoice.Code, cancellationToken);

        var data = new InvoicePdfData(
            invoice.Id,
            invoice.IssuedOn,
            catalogEntry?.Label ?? invoice.Code.Value,
            invoice.Code.Value,
            invoice.Total.Value,
            invoice.PatientShare.Value,
            invoice.MutualityShare.Value,
            invoice.ThirdPartyPayer,
            invoice.Status.ToString(),
            invoice.PaidOn,
            invoice.PaymentMethod,
            practitioner.FullName,
            profession.DisplayName,
            practitioner.InamiNumber,
            practitioner.CabinetName,
            practitioner.CabinetBceNumber,
            practitioner.CabinetAddressLine,
            practitioner.CabinetPostalCode,
            practitioner.CabinetCity,
            patient.FullName,
            patient.AddressLine,
            patient.PostalCode,
            patient.City);

        return Result.Success(InvoicePdfBuilder.Build(data));
    }
}
