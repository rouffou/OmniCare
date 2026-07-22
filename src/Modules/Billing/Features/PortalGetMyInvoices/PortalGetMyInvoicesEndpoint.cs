using Mediarq.AspNetCore;
using Mediarq.Core.Mediators;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using OmniCare.Modules.Billing.Features.ListPatientInvoices;
using OmniCare.SharedKernel.Application;

namespace OmniCare.Modules.Billing.Features.PortalGetMyInvoices;

/// <summary>Portail patient (ticket #39) : consultation de ses propres factures.
/// Réutilise <see cref="ListPatientInvoicesQuery"/> tel quel — seule la résolution du
/// PatientId change, toujours déduite du compte portail authentifié.</summary>
public static class PortalGetMyInvoicesEndpoint
{
    public static IEndpointRouteBuilder MapPortalGetMyInvoices(this IEndpointRouteBuilder app)
    {
        app.MapGet("/api/portal/invoices", async (
            IPatientDirectory patientDirectory,
            ICurrentUserService currentUser,
            ISender sender,
            CancellationToken ct) =>
        {
            var patientId = await patientDirectory.ResolveAsync(currentUser, ct);
            if (patientId is null)
                return Results.Problem(statusCode: 404, title: "not_found", detail: "Aucun dossier patient lié à ce compte.");

            var result = await sender.Send(new ListPatientInvoicesQuery(patientId.Value), ct);
            return result.ToHttpResult();
        })
        .RequireAuthorization(Roles.Patient)
        .WithName("PortalGetMyInvoices")
        .WithTags("Portal");

        return app;
    }
}
