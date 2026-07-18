using Mediarq.AspNetCore;
using Mediarq.Core.Common.Results;
using Mediarq.Core.Mediators;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace OmniCare.Modules.Agenda.Features.GetWaitlist;

public static class GetWaitlistEndpoint
{
    public static IEndpointRouteBuilder MapGetWaitlist(this IEndpointRouteBuilder app)
    {
        app.MapGet("/api/agenda/practitioners/{practitionerId:guid}/waitlist", async (
            Guid practitionerId, ISender sender, CancellationToken ct) =>
        {
            var result = await sender.Send(new GetWaitlistQuery(practitionerId), ct);
            return result.ToHttpResult();
        })
        .WithName("GetWaitlist")
        .WithTags("Agenda");

        return app;
    }
}
