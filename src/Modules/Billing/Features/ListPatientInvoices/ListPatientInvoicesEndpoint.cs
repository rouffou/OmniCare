using Mediarq.AspNetCore;
using Mediarq.Core.Common.Results;
using Mediarq.Core.Mediators;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace OmniCare.Modules.Billing.Features.ListPatientInvoices;

public static class ListPatientInvoicesEndpoint
{
    public static IEndpointRouteBuilder MapListPatientInvoices(this IEndpointRouteBuilder app)
    {
        app.MapGet("/api/billing/patients/{patientId:guid}/invoices", async (
            Guid patientId, ISender sender, CancellationToken ct) =>
        {
            var result = await sender.Send(new ListPatientInvoicesQuery(patientId), ct);
            return result.ToHttpResult();
        })
        .WithName("ListPatientInvoices")
        .WithTags("Billing");

        return app;
    }
}
