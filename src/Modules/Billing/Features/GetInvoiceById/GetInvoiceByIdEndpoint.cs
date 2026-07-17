using Mediarq.AspNetCore;
using Mediarq.Core.Common.Results;
using Mediarq.Core.Mediators;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace OmniCare.Modules.Billing.Features.GetInvoiceById;

public static class GetInvoiceByIdEndpoint
{
    public static IEndpointRouteBuilder MapGetInvoiceById(this IEndpointRouteBuilder app)
    {
        app.MapGet("/api/billing/invoices/{invoiceId:guid}", async (Guid invoiceId, ISender sender, CancellationToken ct) =>
        {
            var result = await sender.Send(new GetInvoiceByIdQuery(invoiceId), ct);
            return result.ToHttpResult();
        })
        .WithName("GetInvoiceById")
        .WithTags("Billing");

        return app;
    }
}
