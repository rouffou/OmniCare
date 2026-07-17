using Mediarq.AspNetCore;
using Mediarq.Core.Common.Results;
using Mediarq.Core.Mediators;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace OmniCare.Modules.Patients.Features.GrantConsent;

public static class GrantConsentEndpoint
{
    public record GrantConsentRequest(string ConsentType);

    public static IEndpointRouteBuilder MapGrantConsent(this IEndpointRouteBuilder app)
    {
        app.MapPost("/api/patients/{patientId:guid}/consents", async (
            Guid patientId,
            GrantConsentRequest body,
            ISender sender,
            CancellationToken ct) =>
        {
            var result = await sender.Send(new GrantConsentCommand(patientId, body.ConsentType), ct);
            return result.ToHttpResult();
        })
        .WithName("GrantConsent")
        .WithTags("Patients");

        return app;
    }
}
