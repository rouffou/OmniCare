using Mediarq.AspNetCore;
using Mediarq.Core.Common.Results;
using Mediarq.Core.Mediators;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace OmniCare.Modules.Agenda.Features.LeaveWaitlist;

public static class LeaveWaitlistEndpoint
{
    public static IEndpointRouteBuilder MapLeaveWaitlist(this IEndpointRouteBuilder app)
    {
        app.MapPost("/api/agenda/waitlist/{waitlistEntryId:guid}/leave", async (
            Guid waitlistEntryId, ISender sender, CancellationToken ct) =>
        {
            var result = await sender.Send(new LeaveWaitlistCommand(waitlistEntryId), ct);
            return result.ToHttpResult();
        })
        .WithName("LeaveWaitlist")
        .WithTags("Agenda");

        return app;
    }
}
