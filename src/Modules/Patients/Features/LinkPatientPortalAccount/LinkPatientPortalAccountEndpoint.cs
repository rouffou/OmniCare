using Mediarq.AspNetCore;
using Mediarq.Core.Mediators;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace OmniCare.Modules.Patients.Features.LinkPatientPortalAccount;

public static class LinkPatientPortalAccountEndpoint
{
    public record LinkPatientPortalAccountRequest(string PortalUserId);

    public static IEndpointRouteBuilder MapLinkPatientPortalAccount(this IEndpointRouteBuilder app)
    {
        app.MapPost("/api/patients/{patientId:guid}/portal-account", async (
            Guid patientId,
            LinkPatientPortalAccountRequest body,
            ISender sender,
            CancellationToken ct) =>
        {
            var result = await sender.Send(
                new LinkPatientPortalAccountCommand(patientId, body.PortalUserId), ct);
            return result.ToHttpResult();
        })
        .WithName("LinkPatientPortalAccount")
        .WithTags("Patients");

        return app;
    }
}
