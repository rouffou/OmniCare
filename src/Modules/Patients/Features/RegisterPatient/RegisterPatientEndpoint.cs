using Mediarq.Core.Common.Results;
using Mediarq.Core.Mediators;
using Mediarq.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace OmniCare.Modules.Patients.Features.RegisterPatient;

public static class RegisterPatientEndpoint
{
    public static IEndpointRouteBuilder MapRegisterPatient(this IEndpointRouteBuilder app)
    {
        app.MapPost("/api/patients", async (RegisterPatientCommand command, ISender sender, CancellationToken ct) =>
        {
            var result = await sender.Send(command, ct);
            return result.ToHttpResult();
        })
        .WithName("RegisterPatient")
        .WithTags("Patients");

        return app;
    }
}
