using Mediarq.Core.Common.Results;
using Mediarq.Core.Mediators;
using Mediarq.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace OmniCare.Modules.Agenda.Features.ChangeAppointmentStatus;

public static class ChangeAppointmentStatusEndpoint
{
    public record ChangeStatusRequest(string Transition);

    public static IEndpointRouteBuilder MapChangeAppointmentStatus(this IEndpointRouteBuilder app)
    {
        app.MapPost("/api/agenda/appointments/{appointmentId:guid}/status", async (
            Guid appointmentId,
            ChangeStatusRequest body,
            ISender sender,
            CancellationToken ct) =>
        {
            var result = await sender.Send(
                new ChangeAppointmentStatusCommand(appointmentId, body.Transition), ct);
            return result.ToHttpResult();
        })
        .WithName("ChangeAppointmentStatus")
        .WithTags("Agenda");

        return app;
    }
}
