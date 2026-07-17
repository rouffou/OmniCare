using Mediarq.Core.Common.Results;
using Mediarq.Core.Mediators;
using Mediarq.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace OmniCare.Modules.Agenda.Features.ScheduleAppointment;

public static class ScheduleAppointmentEndpoint
{
    public static IEndpointRouteBuilder MapScheduleAppointment(this IEndpointRouteBuilder app)
    {
        app.MapPost("/api/agenda/appointments", async (
            ScheduleAppointmentCommand command, ISender sender, CancellationToken ct) =>
        {
            var result = await sender.Send(command, ct);
            return result.ToHttpResult();
        })
        .WithName("ScheduleAppointment")
        .WithTags("Agenda");

        return app;
    }
}
