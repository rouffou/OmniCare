using Mediarq.AspNetCore;
using Mediarq.Core.Mediators;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace OmniCare.Modules.Practitioners.Features.GetPractitionerById;

public static class GetPractitionerByIdEndpoint
{
    public static IEndpointRouteBuilder MapGetPractitionerById(this IEndpointRouteBuilder app)
    {
        app.MapGet("/api/practitioners/{practitionerId:guid}", async (Guid practitionerId, ISender sender, CancellationToken ct) =>
        {
            var result = await sender.Send(new GetPractitionerByIdQuery(practitionerId), ct);
            return result.ToHttpResult();
        })
        .WithName("GetPractitionerById")
        .WithTags("Practitioners");

        return app;
    }
}
