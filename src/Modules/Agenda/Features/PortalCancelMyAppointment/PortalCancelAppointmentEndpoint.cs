using Mediarq.AspNetCore;
using Mediarq.Core.Mediators;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using OmniCare.SharedKernel.Application;

namespace OmniCare.Modules.Agenda.Features.PortalCancelMyAppointment;

public static class PortalCancelAppointmentEndpoint
{
    public record PortalCancelAppointmentRequest(string? Reason);

    public static IEndpointRouteBuilder MapPortalCancelAppointment(this IEndpointRouteBuilder app)
    {
        app.MapPost("/api/portal/appointments/{appointmentId:guid}/cancel", async (
            Guid appointmentId,
            PortalCancelAppointmentRequest body,
            IPatientDirectory patientDirectory,
            ICurrentUserService currentUser,
            ISender sender,
            CancellationToken ct) =>
        {
            var patientId = await patientDirectory.ResolveAsync(currentUser, ct);
            if (patientId is null)
                return Results.Problem(statusCode: 404, title: "not_found", detail: "Aucun dossier patient lié à ce compte.");

            var result = await sender.Send(
                new PortalCancelAppointmentCommand(appointmentId, patientId.Value, body.Reason), ct);
            return result.ToHttpResult();
        })
        .RequireAuthorization(Roles.Patient)
        .WithName("PortalCancelAppointment")
        .WithTags("Portal");

        return app;
    }
}
