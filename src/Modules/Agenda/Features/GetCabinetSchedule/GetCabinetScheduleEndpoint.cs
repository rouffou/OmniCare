using Mediarq.AspNetCore;
using Mediarq.Core.Common.Results;
using Mediarq.Core.Mediators;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace OmniCare.Modules.Agenda.Features.GetCabinetSchedule;

public static class GetCabinetScheduleEndpoint
{
    public static IEndpointRouteBuilder MapGetCabinetSchedule(this IEndpointRouteBuilder app)
    {
        app.MapGet("/api/agenda/appointments", async (
            DateTimeOffset from,
            DateTimeOffset to,
            ISender sender,
            CancellationToken ct) =>
        {
            var result = await sender.Send(new GetCabinetScheduleQuery(from, to), ct);
            return result.ToHttpResult();
        })
        .WithName("GetCabinetSchedule")
        .WithTags("Agenda");

        return app;
    }
}
