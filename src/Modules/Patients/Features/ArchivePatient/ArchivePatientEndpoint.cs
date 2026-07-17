using Mediarq.AspNetCore;
using Mediarq.Core.Common.Results;
using Mediarq.Core.Mediators;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace OmniCare.Modules.Patients.Features.ArchivePatient;

public static class ArchivePatientEndpoint
{
    public static IEndpointRouteBuilder MapArchivePatient(this IEndpointRouteBuilder app)
    {
        app.MapPost("/api/patients/{patientId:guid}/archive", async (
            Guid patientId, ISender sender, CancellationToken ct) =>
        {
            var result = await sender.Send(new ArchivePatientCommand(patientId), ct);
            return result.ToHttpResult();
        })
        .WithName("ArchivePatient")
        .WithTags("Patients");

        return app;
    }
}
