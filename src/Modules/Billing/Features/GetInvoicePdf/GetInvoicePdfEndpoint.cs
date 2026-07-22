using Mediarq.AspNetCore;
using Mediarq.Core.Mediators;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace OmniCare.Modules.Billing.Features.GetInvoicePdf;

public static class GetInvoicePdfEndpoint
{
    public static IEndpointRouteBuilder MapGetInvoicePdf(this IEndpointRouteBuilder app)
    {
        app.MapGet("/api/billing/invoices/{invoiceId:guid}/pdf", async (Guid invoiceId, ISender sender, CancellationToken ct) =>
        {
            var result = await sender.Send(new GetInvoicePdfQuery(invoiceId), ct);
            if (!result.IsSuccess)
                return result.ToHttpResult();

            return Results.File(result.Value, "application/pdf", $"facture-{invoiceId}.pdf");
        })
        .WithName("GetInvoicePdf")
        .WithTags("Billing");

        return app;
    }
}
