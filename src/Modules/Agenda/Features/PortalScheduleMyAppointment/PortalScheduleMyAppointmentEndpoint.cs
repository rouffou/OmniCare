using Mediarq.AspNetCore;
using Mediarq.Core.Mediators;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using OmniCare.Modules.Agenda.Features.ScheduleAppointment;
using OmniCare.SharedKernel.Application;

namespace OmniCare.Modules.Agenda.Features.PortalScheduleMyAppointment;

/// <summary>
/// Portail patient (ticket #39) : prise de RDV en ligne. Réutilise
/// <see cref="ScheduleAppointmentCommand"/> tel quel — le PatientId n'est jamais accepté
/// depuis le corps de la requête, uniquement résolu depuis le compte authentifié, un
/// patient ne pouvant jamais réserver un créneau pour un autre.
/// </summary>
public static class PortalScheduleMyAppointmentEndpoint
{
    public record PortalScheduleMyAppointmentRequest(
        Guid PractitionerId, Guid AppointmentTypeId, DateTimeOffset StartUtc, DateTimeOffset? EndUtc, string? Notes);

    public static IEndpointRouteBuilder MapPortalScheduleMyAppointment(this IEndpointRouteBuilder app)
    {
        app.MapPost("/api/portal/appointments", async (
            PortalScheduleMyAppointmentRequest body,
            IPatientDirectory patientDirectory,
            ICurrentUserService currentUser,
            ISender sender,
            CancellationToken ct) =>
        {
            var patientId = await patientDirectory.ResolveAsync(currentUser, ct);
            if (patientId is null)
                return Results.Problem(statusCode: 404, title: "not_found", detail: "Aucun dossier patient lié à ce compte.");

            var command = new ScheduleAppointmentCommand(
                body.PractitionerId, patientId.Value, body.AppointmentTypeId, body.StartUtc, body.EndUtc, body.Notes);
            var result = await sender.Send(command, ct);
            return result.ToHttpResult();
        })
        .RequireAuthorization(Roles.Patient)
        .WithName("PortalScheduleMyAppointment")
        .WithTags("Portal");

        return app;
    }
}
