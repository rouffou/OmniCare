using Mediarq.Core.Common.Requests.Query;
using Mediarq.Core.Common.Results;
using Microsoft.EntityFrameworkCore;
using OmniCare.Modules.Billing.Infrastructure.Persistence;
using OmniCare.SharedKernel.Application;

namespace OmniCare.Modules.Billing.Features.GetInvoiceById;

public class GetInvoiceByIdHandler : IQueryHandler<GetInvoiceByIdQuery, Result<InvoiceDto>>
{
    private readonly IBillingDbContext _context;

    public GetInvoiceByIdHandler(IBillingDbContext context)
    {
        _context = context;
    }

    public async Task<Result<InvoiceDto>> Handle(GetInvoiceByIdQuery request, CancellationToken cancellationToken = default)
    {
        var invoice = await _context.Invoices
            .AsNoTracking()
            .FirstOrDefaultAsync(i => i.Id == request.InvoiceId, cancellationToken);

        if (invoice is null)
            return BusinessFailures.NotFound<InvoiceDto>($"Facture {request.InvoiceId} introuvable.");

        return Result.Success(new InvoiceDto(
            invoice.Id,
            invoice.PatientId,
            invoice.PractitionerId,
            invoice.Code.Value,
            invoice.Total.Value,
            invoice.InsuredAtIssue,
            invoice.Status.ToString(),
            invoice.IssuedOn,
            invoice.PaidOn,
            invoice.PaymentMethod,
            invoice.CancellationReason));
    }
}
