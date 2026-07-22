using Mediarq.AspNetCore;
using Mediarq.Core.Mediators;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using OmniCare.Modules.Patients.Features.GetClinicalRecord;
using OmniCare.SharedKernel.Application;

namespace OmniCare.Modules.Patients.Features.PortalGetMyClinicalRecord;

/// <summary>
/// Portail patient (ticket #39) : consultation de son propre dossier médical. Réutilise
/// <see cref="GetClinicalRecordQuery"/> tel quel — seule la résolution du PatientId change
/// (jamais fourni par le client, toujours déduit du compte portail authentifié).
/// </summary>
public static class PortalGetMyClinicalRecordEndpoint
{
    public static IEndpointRouteBuilder MapPortalGetMyClinicalRecord(this IEndpointRouteBuilder app)
    {
        app.MapGet("/api/portal/clinical-record", async (
            string profession,
            IPatientDirectory patientDirectory,
            ICurrentUserService currentUser,
            ISender sender,
            CancellationToken ct) =>
        {
            var patientId = await patientDirectory.ResolveAsync(currentUser, ct);
            if (patientId is null)
                return Results.Problem(statusCode: 404, title: "not_found", detail: "Aucun dossier patient lié à ce compte.");

            var result = await sender.Send(new GetClinicalRecordQuery(patientId.Value, profession), ct);
            return result.ToHttpResult();
        })
        .RequireAuthorization(Roles.Patient)
        .WithName("PortalGetMyClinicalRecord")
        .WithTags("Portal");

        return app;
    }
}
