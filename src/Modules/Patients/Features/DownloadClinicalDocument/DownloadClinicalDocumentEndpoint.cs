using Mediarq.AspNetCore;
using Mediarq.Core.Mediators;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace OmniCare.Modules.Patients.Features.DownloadClinicalDocument;

public static class DownloadClinicalDocumentEndpoint
{
    public static IEndpointRouteBuilder MapDownloadClinicalDocument(this IEndpointRouteBuilder app)
    {
        app.MapGet("/api/patients/documents/{documentId:guid}/content", async (
            Guid documentId, ISender sender, CancellationToken ct) =>
        {
            var result = await sender.Send(new DownloadClinicalDocumentQuery(documentId), ct);
            if (!result.IsSuccess)
                return result.ToHttpResult();

            return Results.File(result.Value.Content, result.Value.ContentType, result.Value.FileName);
        })
        .WithName("DownloadClinicalDocument")
        .WithTags("Patients");

        return app;
    }
}
