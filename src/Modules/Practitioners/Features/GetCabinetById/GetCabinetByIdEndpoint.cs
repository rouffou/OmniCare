using Mediarq.AspNetCore;
using Mediarq.Core.Mediators;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace OmniCare.Modules.Practitioners.Features.GetCabinetById;

public static class GetCabinetByIdEndpoint
{
    public static IEndpointRouteBuilder MapGetCabinetById(this IEndpointRouteBuilder app)
    {
        app.MapGet("/api/cabinets/{cabinetId:guid}", async (Guid cabinetId, ISender sender, CancellationToken ct) =>
        {
            var result = await sender.Send(new GetCabinetByIdQuery(cabinetId), ct);
            return result.ToHttpResult();
        })
        .WithName("GetCabinetById")
        .WithTags("Practitioners");

        return app;
    }
}
