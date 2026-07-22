using Mediarq.AspNetCore;
using Mediarq.Core.Mediators;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using OmniCare.SharedKernel.Application;

namespace OmniCare.Modules.Agenda.Features.PortalGetMyAppointments;

public static class PortalGetMyAppointmentsEndpoint
{
    public static IEndpointRouteBuilder MapPortalGetMyAppointments(this IEndpointRouteBuilder app)
    {
        app.MapGet("/api/portal/appointments", async (
            IPatientDirectory patientDirectory,
            ICurrentUserService currentUser,
            ISender sender,
            CancellationToken ct) =>
        {
            var patientId = await patientDirectory.ResolveAsync(currentUser, ct);
            if (patientId is null)
                return Results.Problem(statusCode: 404, title: "not_found", detail: "Aucun dossier patient lié à ce compte.");

            var result = await sender.Send(new PortalGetMyAppointmentsQuery(patientId.Value), ct);
            return result.ToHttpResult();
        })
        .RequireAuthorization(Roles.Patient)
        .WithName("PortalGetMyAppointments")
        .WithTags("Portal");

        return app;
    }
}
