using Mediarq.AspNetCore;
using Mediarq.Core.Common.Results;
using Mediarq.Core.Mediators;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace OmniCare.Modules.Billing.Features.CancelInvoice;

public static class CancelInvoiceEndpoint
{
    public record CancelInvoiceRequest(string? Reason);

    public static IEndpointRouteBuilder MapCancelInvoice(this IEndpointRouteBuilder app)
    {
        app.MapPost("/api/billing/invoices/{invoiceId:guid}/cancel", async (
            Guid invoiceId,
            CancelInvoiceRequest body,
            ISender sender,
            CancellationToken ct) =>
        {
            var result = await sender.Send(new CancelInvoiceCommand(invoiceId, body.Reason), ct);
            return result.ToHttpResult();
        })
        .WithName("CancelInvoice")
        .WithTags("Billing");

        return app;
    }
}
