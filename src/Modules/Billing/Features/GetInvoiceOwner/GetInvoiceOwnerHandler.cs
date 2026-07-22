using Mediarq.Core.Common.Requests.Query;
using Mediarq.Core.Common.Results;
using Microsoft.EntityFrameworkCore;
using OmniCare.Modules.Billing.Infrastructure.Persistence;
using OmniCare.SharedKernel.Application;

namespace OmniCare.Modules.Billing.Features.GetInvoiceOwner;

public class GetInvoiceOwnerHandler : IQueryHandler<GetInvoiceOwnerQuery, Result<Guid>>
{
    private readonly IBillingDbContext _context;

    public GetInvoiceOwnerHandler(IBillingDbContext context)
    {
        _context = context;
    }

    public async Task<Result<Guid>> Handle(GetInvoiceOwnerQuery request, CancellationToken cancellationToken = default)
    {
        var patientId = await _context.Invoices
            .AsNoTracking()
            .Where(i => i.Id == request.InvoiceId)
            .Select(i => (Guid?)i.PatientId)
            .FirstOrDefaultAsync(cancellationToken);

        if (patientId is null)
            return BusinessFailures.NotFound<Guid>($"Facture {request.InvoiceId} introuvable.");

        return Result.Success(patientId.Value);
    }
}
