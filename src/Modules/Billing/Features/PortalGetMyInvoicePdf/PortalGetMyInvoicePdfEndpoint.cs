using Mediarq.AspNetCore;
using Mediarq.Core.Mediators;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using OmniCare.Modules.Billing.Features.GetInvoiceOwner;
using OmniCare.Modules.Billing.Features.GetInvoicePdf;
using OmniCare.SharedKernel.Application;

namespace OmniCare.Modules.Billing.Features.PortalGetMyInvoicePdf;

/// <summary>Portail patient (ticket #39) : téléchargement du PDF d'une de ses propres
/// factures. Vérifie l'appartenance via <see cref="GetInvoiceOwnerQuery"/> avant de
/// déléguer à <see cref="GetInvoicePdfQuery"/> (accès praticien/secrétariat, aucun
/// scoping) — un patient ne doit jamais pouvoir lire la facture d'un autre.</summary>
public static class PortalGetMyInvoicePdfEndpoint
{
    public static IEndpointRouteBuilder MapPortalGetMyInvoicePdf(this IEndpointRouteBuilder app)
    {
        app.MapGet("/api/portal/invoices/{invoiceId:guid}/pdf", async (
            Guid invoiceId,
            IPatientDirectory patientDirectory,
            ICurrentUserService currentUser,
            ISender sender,
            CancellationToken ct) =>
        {
            var patientId = await patientDirectory.ResolveAsync(currentUser, ct);
            if (patientId is null)
                return Results.Problem(statusCode: 404, title: "not_found", detail: "Aucun dossier patient lié à ce compte.");

            var ownerResult = await sender.Send(new GetInvoiceOwnerQuery(invoiceId), ct);
            // Même message pour "inexistant" et "appartient à un autre patient" : ne jamais
            // révéler à un patient qu'une facture identifiée par cet id existe ailleurs.
            if (!ownerResult.IsSuccess || ownerResult.Value != patientId.Value)
                return Results.Problem(statusCode: 404, title: "not_found", detail: $"Facture {invoiceId} introuvable.");

            var pdfResult = await sender.Send(new GetInvoicePdfQuery(invoiceId), ct);
            if (!pdfResult.IsSuccess)
                return pdfResult.ToHttpResult();

            return Results.File(pdfResult.Value, "application/pdf", $"facture-{invoiceId}.pdf");
        })
        .RequireAuthorization(Roles.Patient)
        .WithName("PortalGetMyInvoicePdf")
        .WithTags("Portal");

        return app;
    }
}
