using Mediarq.AspNetCore;
using Mediarq.Core.Mediators;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace OmniCare.Modules.Patients.Features.RegisterClinicalDocument;

public static class RegisterClinicalDocumentEndpoint
{
    public record RegisterClinicalDocumentRequest(
        string ProfessionCode,
        string DocumentType,
        string FileName,
        string ContentType,
        string ContentBase64,
        Guid UploadedByPractitionerId);

    public static IEndpointRouteBuilder MapRegisterClinicalDocument(this IEndpointRouteBuilder app)
    {
        app.MapPost("/api/patients/{patientId:guid}/clinical-record/documents", async (
            Guid patientId,
            RegisterClinicalDocumentRequest body,
            ISender sender,
            CancellationToken ct) =>
        {
            var command = new RegisterClinicalDocumentCommand(
                patientId, body.ProfessionCode, body.DocumentType, body.FileName, body.ContentType,
                body.ContentBase64, body.UploadedByPractitionerId);
            var result = await sender.Send(command, ct);
            return result.ToHttpResult();
        })
        .WithName("RegisterClinicalDocument")
        .WithTags("Patients");

        return app;
    }
}
