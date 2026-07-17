using Mediarq.Core.Common.Results;
using Mediarq.Core.Mediators;
using Mediarq.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace OmniCare.Modules.Patients.Features.GetPatientById;

public static class GetPatientByIdEndpoint
{
    public static IEndpointRouteBuilder MapGetPatientById(this IEndpointRouteBuilder app)
    {
        app.MapGet("/api/patients/{patientId:guid}", async (Guid patientId, ISender sender, CancellationToken ct) =>
        {
            var result = await sender.Send(new GetPatientByIdQuery(patientId), ct);
            return result.ToHttpResult();
        })
        .WithName("GetPatientById")
        .WithTags("Patients");

        return app;
    }
}
