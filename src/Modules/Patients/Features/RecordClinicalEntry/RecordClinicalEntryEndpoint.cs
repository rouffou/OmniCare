using Mediarq.Core.Common.Results;
using Mediarq.Core.Mediators;
using Mediarq.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace OmniCare.Modules.Patients.Features.RecordClinicalEntry;

public static class RecordClinicalEntryEndpoint
{
    public record RecordClinicalEntryRequest(
        string ProfessionCode,
        string EntryType,
        string Content,
        Guid AuthorPractitionerId,
        string? Title);

    public static IEndpointRouteBuilder MapRecordClinicalEntry(this IEndpointRouteBuilder app)
    {
        app.MapPost("/api/patients/{patientId:guid}/clinical-record/entries", async (
            Guid patientId,
            RecordClinicalEntryRequest body,
            ISender sender,
            CancellationToken ct) =>
        {
            var command = new RecordClinicalEntryCommand(
                patientId, body.ProfessionCode, body.EntryType, body.Content,
                body.AuthorPractitionerId, body.Title);
            var result = await sender.Send(command, ct);
            return result.ToHttpResult();
        })
        .WithName("RecordClinicalEntry")
        .WithTags("Patients");

        return app;
    }
}
