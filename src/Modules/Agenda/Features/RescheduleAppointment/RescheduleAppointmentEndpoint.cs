using Mediarq.AspNetCore;
using Mediarq.Core.Common.Results;
using Mediarq.Core.Mediators;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace OmniCare.Modules.Agenda.Features.RescheduleAppointment;

public static class RescheduleAppointmentEndpoint
{
    public record RescheduleRequest(DateTimeOffset StartUtc, DateTimeOffset? EndUtc);

    public static IEndpointRouteBuilder MapRescheduleAppointment(this IEndpointRouteBuilder app)
    {
        app.MapPost("/api/agenda/appointments/{appointmentId:guid}/reschedule", async (
            Guid appointmentId,
            RescheduleRequest body,
            ISender sender,
            CancellationToken ct) =>
        {
            var command = new RescheduleAppointmentCommand(appointmentId, body.StartUtc, body.EndUtc);
            var result = await sender.Send(command, ct);
            return result.ToHttpResult();
        })
        .WithName("RescheduleAppointment")
        .WithTags("Agenda");

        return app;
    }
}
