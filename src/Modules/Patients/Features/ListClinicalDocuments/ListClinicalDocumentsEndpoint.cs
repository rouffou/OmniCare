using Mediarq.AspNetCore;
using Mediarq.Core.Mediators;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace OmniCare.Modules.Patients.Features.ListClinicalDocuments;

public static class ListClinicalDocumentsEndpoint
{
    public static IEndpointRouteBuilder MapListClinicalDocuments(this IEndpointRouteBuilder app)
    {
        app.MapGet("/api/patients/{patientId:guid}/clinical-record/documents", async (
            Guid patientId,
            string professionCode,
            ISender sender,
            CancellationToken ct) =>
        {
            var result = await sender.Send(new ListClinicalDocumentsQuery(patientId, professionCode), ct);
            return result.ToHttpResult();
        })
        .WithName("ListClinicalDocuments")
        .WithTags("Patients");

        return app;
    }
}
