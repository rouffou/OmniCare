using Mediarq.AspNetCore;
using Mediarq.Core.Common.Results;
using Mediarq.Core.Mediators;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace OmniCare.Modules.Billing.Features.DefineActCatalogEntry;

public static class DefineActCatalogEntryEndpoint
{
    public static IEndpointRouteBuilder MapDefineActCatalogEntry(this IEndpointRouteBuilder app)
    {
        app.MapPost("/api/billing/act-catalog", async (
            DefineActCatalogEntryCommand command, ISender sender, CancellationToken ct) =>
        {
            var result = await sender.Send(command, ct);
            return result.ToHttpResult();
        })
        .WithName("DefineActCatalogEntry")
        .WithTags("Billing");

        return app;
    }
}
