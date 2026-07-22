using Mediarq.AspNetCore;
using Mediarq.Core.Mediators;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using OmniCare.SharedKernel.Application;

namespace OmniCare.Modules.Patients.Features.PortalDownloadMyClinicalDocument;

public static class PortalDownloadClinicalDocumentEndpoint
{
    public static IEndpointRouteBuilder MapPortalDownloadClinicalDocument(this IEndpointRouteBuilder app)
    {
        app.MapGet("/api/portal/documents/{documentId:guid}/content", async (
            Guid documentId,
            IPatientDirectory patientDirectory,
            ICurrentUserService currentUser,
            ISender sender,
            CancellationToken ct) =>
        {
            var patientId = await patientDirectory.ResolveAsync(currentUser, ct);
            if (patientId is null)
                return Results.Problem(statusCode: 404, title: "not_found", detail: "Aucun dossier patient lié à ce compte.");

            var result = await sender.Send(
                new PortalDownloadClinicalDocumentQuery(documentId, patientId.Value), ct);
            if (!result.IsSuccess)
                return result.ToHttpResult();

            return Results.File(result.Value.Content, result.Value.ContentType, result.Value.FileName);
        })
        .RequireAuthorization(Roles.Patient)
        .WithName("PortalDownloadClinicalDocument")
        .WithTags("Portal");

        return app;
    }
}
