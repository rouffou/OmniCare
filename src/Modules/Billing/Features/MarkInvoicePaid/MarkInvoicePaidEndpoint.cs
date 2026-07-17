using Mediarq.AspNetCore;
using Mediarq.Core.Common.Results;
using Mediarq.Core.Mediators;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace OmniCare.Modules.Billing.Features.MarkInvoicePaid;

public static class MarkInvoicePaidEndpoint
{
    public record MarkPaidRequest(string PaymentMethod);

    public static IEndpointRouteBuilder MapMarkInvoicePaid(this IEndpointRouteBuilder app)
    {
        app.MapPost("/api/billing/invoices/{invoiceId:guid}/pay", async (
            Guid invoiceId,
            MarkPaidRequest body,
            ISender sender,
            CancellationToken ct) =>
        {
            var result = await sender.Send(new MarkInvoicePaidCommand(invoiceId, body.PaymentMethod), ct);
            return result.ToHttpResult();
        })
        .WithName("MarkInvoicePaid")
        .WithTags("Billing");

        return app;
    }
}
