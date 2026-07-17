using Mediarq.Core.Common.Results;
using Mediarq.Core.Mediators;
using Mediarq.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace OmniCare.Modules.Agenda.Features.DefineAppointmentType;

public static class DefineAppointmentTypeEndpoint
{
    public static IEndpointRouteBuilder MapDefineAppointmentType(this IEndpointRouteBuilder app)
    {
        app.MapPost("/api/agenda/appointment-types", async (
            DefineAppointmentTypeCommand command, ISender sender, CancellationToken ct) =>
        {
            var result = await sender.Send(command, ct);
            return result.ToHttpResult();
        })
        .WithName("DefineAppointmentType")
        .WithTags("Agenda");

        return app;
    }
}
