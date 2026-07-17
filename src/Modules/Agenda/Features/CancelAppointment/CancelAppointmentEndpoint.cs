using Mediarq.Core.Common.Results;
using Mediarq.Core.Mediators;
using Mediarq.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace OmniCare.Modules.Agenda.Features.CancelAppointment;

public static class CancelAppointmentEndpoint
{
    public record CancelAppointmentRequest(string? Reason);

    public static IEndpointRouteBuilder MapCancelAppointment(this IEndpointRouteBuilder app)
    {
        app.MapPost("/api/agenda/appointments/{appointmentId:guid}/cancel", async (
            Guid appointmentId,
            CancelAppointmentRequest body,
            ISender sender,
            CancellationToken ct) =>
        {
            var result = await sender.Send(new CancelAppointmentCommand(appointmentId, body.Reason), ct);
            return result.ToHttpResult();
        })
        .WithName("CancelAppointment")
        .WithTags("Agenda");

        return app;
    }
}
