using Mediarq.AspNetCore;
using Mediarq.Core.Common.Results;
using Mediarq.Core.Mediators;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace OmniCare.Modules.Billing.Features.RetryInvoiceTransmission;

public static class RetryInvoiceTransmissionEndpoint
{
    public static IEndpointRouteBuilder MapRetryInvoiceTransmission(this IEndpointRouteBuilder app)
    {
        app.MapPost("/api/billing/invoices/{invoiceId:guid}/retry-transmission", async (
            Guid invoiceId, ISender sender, CancellationToken ct) =>
        {
            var result = await sender.Send(new RetryInvoiceTransmissionCommand(invoiceId), ct);
            return result.ToHttpResult();
        })
        .WithName("RetryInvoiceTransmission")
        .WithTags("Billing");

        return app;
    }
}
