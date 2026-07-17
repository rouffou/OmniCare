using Mediarq.Core.Common.Requests.Query;
using Mediarq.Core.Common.Results;

namespace OmniCare.Modules.Billing.Features.GetInvoiceById;

public record GetInvoiceByIdQuery(Guid InvoiceId) : IQuery<Result<InvoiceDto>>;

public record InvoiceDto(
    Guid Id,
    Guid PatientId,
    Guid PractitionerId,
    string InamiCode,
    decimal Total,
    bool InsuredAtIssue,
    string Status,
    DateTimeOffset IssuedOn,
    DateTimeOffset? PaidOn,
    string? PaymentMethod,
    string? CancellationReason);
