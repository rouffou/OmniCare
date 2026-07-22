using Mediarq.AspNetCore;
using Mediarq.Core.Mediators;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace OmniCare.Modules.Practitioners.Features.RegisterCabinet;

public static class RegisterCabinetEndpoint
{
    public static IEndpointRouteBuilder MapRegisterCabinet(this IEndpointRouteBuilder app)
    {
        app.MapPost("/api/cabinets", async (RegisterCabinetCommand command, ISender sender, CancellationToken ct) =>
        {
            var result = await sender.Send(command, ct);
            return result.ToHttpResult();
        })
        .WithName("RegisterCabinet")
        .WithTags("Practitioners");

        return app;
    }
}
