using Mediarq.Core.Common.Results;
using Mediarq.Core.Mediators;
using Mediarq.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace OmniCare.Modules.Patients.Features.RegisterPrescription;

public static class RegisterPrescriptionEndpoint
{
    public record RegisterPrescriptionRequest(
        string ProfessionCode,
        string PrescriberName,
        DateOnly PrescribedOn,
        int SessionsPrescribed);

    public static IEndpointRouteBuilder MapRegisterPrescription(this IEndpointRouteBuilder app)
    {
        app.MapPost("/api/patients/{patientId:guid}/clinical-record/prescriptions", async (
            Guid patientId,
            RegisterPrescriptionRequest body,
            ISender sender,
            CancellationToken ct) =>
        {
            var command = new RegisterPrescriptionCommand(
                patientId, body.ProfessionCode, body.PrescriberName,
                body.PrescribedOn, body.SessionsPrescribed);
            var result = await sender.Send(command, ct);
            return result.ToHttpResult();
        })
        .WithName("RegisterPrescription")
        .WithTags("Patients");

        return app;
    }
}
