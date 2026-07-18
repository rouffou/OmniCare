using Mediarq.AspNetCore;
using Mediarq.Core.Common.Results;
using Mediarq.Core.Mediators;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace OmniCare.Modules.Agenda.Features.DefineRoom;

public static class DefineRoomEndpoint
{
    public static IEndpointRouteBuilder MapDefineRoom(this IEndpointRouteBuilder app)
    {
        app.MapPost("/api/agenda/rooms", async (
            DefineRoomCommand command, ISender sender, CancellationToken ct) =>
        {
            var result = await sender.Send(command, ct);
            return result.ToHttpResult();
        })
        .WithName("DefineRoom")
        .WithTags("Agenda");

        return app;
    }
}
