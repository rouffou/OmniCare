using Mediarq.Core.Common.Requests.Query;
using Mediarq.Core.Common.Results;
using Microsoft.EntityFrameworkCore;
using OmniCare.Modules.Billing.Domain.Entities;
using OmniCare.Modules.Billing.Infrastructure.Persistence;

namespace OmniCare.Modules.Billing.Features.ListPatientInvoices;

public class ListPatientInvoicesHandler
    : IQueryHandler<ListPatientInvoicesQuery, Result<PatientInvoicesDto>>
{
    private readonly IBillingDbContext _context;

    public ListPatientInvoicesHandler(IBillingDbContext context)
    {
        _context = context;
    }

    public async Task<Result<PatientInvoicesDto>> Handle(ListPatientInvoicesQuery request, CancellationToken cancellationToken = default)
    {
        var invoices = await _context.Invoices
            .AsNoTracking()
            .Where(i => i.PatientId == request.PatientId)
            .OrderByDescending(i => i.IssuedOn)
            .ToListAsync(cancellationToken);

        // Les annulées ne comptent pas dans l'état de compte.
        var issued = invoices.Where(i => i.Status != InvoiceStatus.Cancelled).ToList();
        var totalIssued = issued.Sum(i => i.Total.Value);
        var totalPaid = issued.Where(i => i.Status == InvoiceStatus.Paid).Sum(i => i.Total.Value);

        var dto = new PatientInvoicesDto(
            request.PatientId,
            totalIssued,
            totalPaid,
            totalIssued - totalPaid,
            invoices.Select(i => new InvoiceSummaryDto(
                i.Id, i.Code.Value, i.Total.Value, i.Status.ToString(), i.IssuedOn, i.PaidOn))
                .ToList());

        return Result.Success(dto);
    }
}
