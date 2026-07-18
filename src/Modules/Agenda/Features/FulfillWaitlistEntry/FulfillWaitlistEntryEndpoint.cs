using Mediarq.AspNetCore;
using Mediarq.Core.Common.Results;
using Mediarq.Core.Mediators;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace OmniCare.Modules.Agenda.Features.FulfillWaitlistEntry;

public static class FulfillWaitlistEntryEndpoint
{
    public static IEndpointRouteBuilder MapFulfillWaitlistEntry(this IEndpointRouteBuilder app)
    {
        app.MapPost("/api/agenda/waitlist/{waitlistEntryId:guid}/fulfill", async (
            Guid waitlistEntryId, ISender sender, CancellationToken ct) =>
        {
            var result = await sender.Send(new FulfillWaitlistEntryCommand(waitlistEntryId), ct);
            return result.ToHttpResult();
        })
        .WithName("FulfillWaitlistEntry")
        .WithTags("Agenda");

        return app;
    }
}
