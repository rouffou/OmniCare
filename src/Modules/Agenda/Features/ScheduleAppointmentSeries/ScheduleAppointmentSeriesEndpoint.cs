using Mediarq.AspNetCore;
using Mediarq.Core.Common.Results;
using Mediarq.Core.Mediators;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace OmniCare.Modules.Agenda.Features.ScheduleAppointmentSeries;

public static class ScheduleAppointmentSeriesEndpoint
{
    public static IEndpointRouteBuilder MapScheduleAppointmentSeries(this IEndpointRouteBuilder app)
    {
        app.MapPost("/api/agenda/appointments/series", async (
            ScheduleAppointmentSeriesCommand command, ISender sender, CancellationToken ct) =>
        {
            var result = await sender.Send(command, ct);
            return result.ToHttpResult();
        })
        .WithName("ScheduleAppointmentSeries")
        .WithTags("Agenda");

        return app;
    }
}
