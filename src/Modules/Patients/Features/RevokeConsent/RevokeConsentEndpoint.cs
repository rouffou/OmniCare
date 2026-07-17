using Mediarq.AspNetCore;
using Mediarq.Core.Common.Results;
using Mediarq.Core.Mediators;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace OmniCare.Modules.Patients.Features.RevokeConsent;

public static class RevokeConsentEndpoint
{
    public static IEndpointRouteBuilder MapRevokeConsent(this IEndpointRouteBuilder app)
    {
        // POST plutôt que DELETE : la révocation n'efface rien, elle horodate le retrait
        // (l'historique des consentements est conservé).
        app.MapPost("/api/patients/{patientId:guid}/consents/{consentType}/revoke", async (
            Guid patientId,
            string consentType,
            ISender sender,
            CancellationToken ct) =>
        {
            var result = await sender.Send(new RevokeConsentCommand(patientId, consentType), ct);
            return result.ToHttpResult();
        })
        .WithName("RevokeConsent")
        .WithTags("Patients");

        return app;
    }
}
