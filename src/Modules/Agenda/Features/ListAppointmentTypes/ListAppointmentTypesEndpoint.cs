using Mediarq.AspNetCore;
using Mediarq.Core.Mediators;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace OmniCare.Modules.Agenda.Features.ListAppointmentTypes;

public static class ListAppointmentTypesEndpoint
{
    public static IEndpointRouteBuilder MapListAppointmentTypes(this IEndpointRouteBuilder app)
    {
        app.MapGet("/api/agenda/appointment-types", async (
            string? professionCode, ISender sender, CancellationToken ct) =>
        {
            var result = await sender.Send(new ListAppointmentTypesQuery(professionCode), ct);
            return result.ToHttpResult();
        })
        .WithName("ListAppointmentTypes")
        .WithTags("Agenda");

        return app;
    }
}
