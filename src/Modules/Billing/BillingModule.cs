using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using OmniCare.Modules.Billing.Features.CancelInvoice;
using OmniCare.Modules.Billing.Features.DefineActCatalogEntry;
using OmniCare.Modules.Billing.Features.GenerateInvoice;
using OmniCare.Modules.Billing.Features.GetInvoiceById;
using OmniCare.Modules.Billing.Features.GetInvoicePdf;
using OmniCare.Modules.Billing.Features.ListPatientInvoices;
using OmniCare.Modules.Billing.Features.MarkInvoicePaid;
using OmniCare.Modules.Billing.Features.RetryInvoiceTransmission;
using OmniCare.Modules.Billing.Infrastructure.Persistence;
using OmniCare.Modules.Billing.Infrastructure.Services;

namespace OmniCare.Modules.Billing;

/// <summary>Composition root du module : persistance, services externes, endpoints.</summary>
public static class BillingModule
{
    public static IServiceCollection AddBillingModule(
        this IServiceCollection services,
        Action<DbContextOptionsBuilder> configureDb)
    {
        services.AddDbContext<BillingDbContext>(configureDb);
        services.AddScoped<IBillingDbContext>(sp => sp.GetRequiredService<BillingDbContext>());
        // Client MyCareNet factice en attendant l'intégration réelle (Phase 2, Mediarq.Polly).
        services.AddScoped<IMyCareNetService, FakeMyCareNetService>();
        // Idem pour la télétransmission eAttest — accepte systématiquement en attendant l'accès eHealth réel.
        services.AddScoped<IEHealthTransmissionService, FakeEHealthTransmissionService>();
        return services;
    }

    public static IEndpointRouteBuilder MapBillingModule(this IEndpointRouteBuilder app)
    {
        app.MapDefineActCatalogEntry();
        app.MapGenerateInvoice();
        app.MapGetInvoiceById();
        app.MapGetInvoicePdf();
        app.MapMarkInvoicePaid();
        app.MapCancelInvoice();
        app.MapListPatientInvoices();
        app.MapRetryInvoiceTransmission();
        return app;
    }
}
