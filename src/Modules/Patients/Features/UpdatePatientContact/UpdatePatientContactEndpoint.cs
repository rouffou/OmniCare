using Mediarq.AspNetCore;
using Mediarq.Core.Common.Results;
using Mediarq.Core.Mediators;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace OmniCare.Modules.Patients.Features.UpdatePatientContact;

public static class UpdatePatientContactEndpoint
{
    public record UpdateContactRequest(
        string? Email,
        string? Phone,
        string? AddressLine,
        string? PostalCode,
        string? City);

    public static IEndpointRouteBuilder MapUpdatePatientContact(this IEndpointRouteBuilder app)
    {
        app.MapPut("/api/patients/{patientId:guid}/contact", async (
            Guid patientId,
            UpdateContactRequest body,
            ISender sender,
            CancellationToken ct) =>
        {
            var command = new UpdatePatientContactCommand(
                patientId, body.Email, body.Phone, body.AddressLine, body.PostalCode, body.City);
            var result = await sender.Send(command, ct);
            return result.ToHttpResult();
        })
        .WithName("UpdatePatientContact")
        .WithTags("Patients");

        return app;
    }
}
