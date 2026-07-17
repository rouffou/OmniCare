using Mediarq.Core.Common.Results;
using Mediarq.Core.Mediators;
using Mediarq.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace OmniCare.Modules.Patients.Features.SearchPatients;

public static class SearchPatientsEndpoint
{
    public static IEndpointRouteBuilder MapSearchPatients(this IEndpointRouteBuilder app)
    {
        app.MapGet("/api/patients", async (
            string? search,
            string? status,
            Guid? referentPractitionerId,
            int? page,
            int? pageSize,
            ISender sender,
            CancellationToken ct) =>
        {
            var query = new SearchPatientsQuery(
                search,
                status,
                referentPractitionerId,
                page ?? 1,
                pageSize ?? 20);
            var result = await sender.Send(query, ct);
            return result.ToHttpResult();
        })
        .WithName("SearchPatients")
        .WithTags("Patients");

        return app;
    }
}
