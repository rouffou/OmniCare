using Mediarq.AspNetCore;
using Mediarq.Core.Common.Results;
using Mediarq.Core.Mediators;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace OmniCare.Modules.Billing.Features.GenerateInvoice;

public static class GenerateInvoiceEndpoint
{
    public record GenerateInvoiceRequest(
        Guid PatientId,
        Guid PractitionerId,
        string ProfessionCode,
        string InamiCodeStr,
        decimal BaseAmount,
        decimal? PatientShareAmount = null,
        bool ThirdPartyPayer = false);

    public static IEndpointRouteBuilder MapGenerateInvoice(this IEndpointRouteBuilder app)
    {
        app.MapPost("/api/billing/invoices", async (
            GenerateInvoiceRequest body,
            HttpRequest http,
            ISender sender,
            CancellationToken ct) =>
        {
            // Clé d'idempotence fournie par l'appelant (protection contre les doubles
            // émissions, ex. double-clic ou retry réseau).
            var idempotencyKey = http.Headers["Idempotency-Key"].FirstOrDefault()
                ?? Guid.NewGuid().ToString("N");

            var command = new GenerateInvoiceCommand(
                body.PatientId, body.PractitionerId, body.ProfessionCode, body.InamiCodeStr, body.BaseAmount,
                idempotencyKey, body.PatientShareAmount, body.ThirdPartyPayer);
            var result = await sender.Send(command, ct);
            return result.ToHttpResult();
        })
        .WithName("GenerateInvoice")
        .WithTags("Billing");

        return app;
    }
}
