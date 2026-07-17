using Mediarq.Core.Common.Results;
using Mediarq.Core.Mediators;
using Mediarq.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace OmniCare.Modules.Patients.Features.GetClinicalRecord;

public static class GetClinicalRecordEndpoint
{
    public static IEndpointRouteBuilder MapGetClinicalRecord(this IEndpointRouteBuilder app)
    {
        app.MapGet("/api/patients/{patientId:guid}/clinical-record", async (
            Guid patientId,
            string profession,
            ISender sender,
            CancellationToken ct) =>
        {
            var result = await sender.Send(new GetClinicalRecordQuery(patientId, profession), ct);
            return result.ToHttpResult();
        })
        .WithName("GetClinicalRecord")
        .WithTags("Patients");

        return app;
    }
}
