using Mediarq.Core.Common.Results;
using Mediarq.Core.Mediators;
using Mediarq.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace OmniCare.Modules.Agenda.Features.GetPractitionerSchedule;

public static class GetPractitionerScheduleEndpoint
{
    public static IEndpointRouteBuilder MapGetPractitionerSchedule(this IEndpointRouteBuilder app)
    {
        app.MapGet("/api/agenda/practitioners/{practitionerId:guid}/appointments", async (
            Guid practitionerId,
            DateTimeOffset from,
            DateTimeOffset to,
            ISender sender,
            CancellationToken ct) =>
        {
            var result = await sender.Send(
                new GetPractitionerScheduleQuery(practitionerId, from, to), ct);
            return result.ToHttpResult();
        })
        .WithName("GetPractitionerSchedule")
        .WithTags("Agenda");

        return app;
    }
}
