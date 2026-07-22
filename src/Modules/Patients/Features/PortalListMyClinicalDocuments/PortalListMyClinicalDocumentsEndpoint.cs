using Mediarq.AspNetCore;
using Mediarq.Core.Mediators;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using OmniCare.Modules.Patients.Features.ListClinicalDocuments;
using OmniCare.SharedKernel.Application;

namespace OmniCare.Modules.Patients.Features.PortalListMyClinicalDocuments;

/// <summary>Portail patient (ticket #39) : liste de ses propres documents joints.</summary>
public static class PortalListMyClinicalDocumentsEndpoint
{
    public static IEndpointRouteBuilder MapPortalListMyClinicalDocuments(this IEndpointRouteBuilder app)
    {
        app.MapGet("/api/portal/clinical-record/documents", async (
            string professionCode,
            IPatientDirectory patientDirectory,
            ICurrentUserService currentUser,
            ISender sender,
            CancellationToken ct) =>
        {
            var patientId = await patientDirectory.ResolveAsync(currentUser, ct);
            if (patientId is null)
                return Results.Problem(statusCode: 404, title: "not_found", detail: "Aucun dossier patient lié à ce compte.");

            var result = await sender.Send(new ListClinicalDocumentsQuery(patientId.Value, professionCode), ct);
            return result.ToHttpResult();
        })
        .RequireAuthorization(Roles.Patient)
        .WithName("PortalListMyClinicalDocuments")
        .WithTags("Portal");

        return app;
    }
}
