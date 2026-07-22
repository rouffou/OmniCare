using Mediarq.Core.Common.Requests.Query;
using Mediarq.Core.Common.Results;

namespace OmniCare.Modules.Billing.Features.GetInvoicePdf;

public record GetInvoicePdfQuery(Guid InvoiceId) : IQuery<Result<byte[]>>;
