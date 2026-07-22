using Mediarq.AspNetCore;
using Mediarq.Core.Mediators;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace OmniCare.Modules.Practitioners.Features.RegisterPractitioner;

public static class RegisterPractitionerEndpoint
{
    public static IEndpointRouteBuilder MapRegisterPractitioner(this IEndpointRouteBuilder app)
    {
        app.MapPost("/api/practitioners", async (RegisterPractitionerCommand command, ISender sender, CancellationToken ct) =>
        {
            var result = await sender.Send(command, ct);
            return result.ToHttpResult();
        })
        .WithName("RegisterPractitioner")
        .WithTags("Practitioners");

        return app;
    }
}
