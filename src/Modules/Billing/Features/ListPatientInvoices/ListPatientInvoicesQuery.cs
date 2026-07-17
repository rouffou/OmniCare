using Mediarq.Core.Common.Requests.Query;
using Mediarq.Core.Common.Results;

namespace OmniCare.Modules.Billing.Features.ListPatientInvoices;

/// <summary>
/// État de compte d'un patient : factures triées de la plus récente à la plus
/// ancienne, avec totaux émis/payé/en attente (cahier des charges §4.3, suivi
/// des paiements).
/// </summary>
public record ListPatientInvoicesQuery(Guid PatientId)
    : IQuery<Result<PatientInvoicesDto>>;

public record PatientInvoicesDto(
    Guid PatientId,
    decimal TotalIssued,
    decimal TotalPaid,
    decimal TotalOutstanding,
    IReadOnlyList<InvoiceSummaryDto> Invoices);

public record InvoiceSummaryDto(
    Guid Id,
    string InamiCode,
    decimal Total,
    string Status,
    DateTimeOffset IssuedOn,
    DateTimeOffset? PaidOn);
