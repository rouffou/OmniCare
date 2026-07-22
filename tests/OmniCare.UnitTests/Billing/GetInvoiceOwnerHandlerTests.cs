using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using OmniCare.Modules.Billing.Domain.Entities;
using OmniCare.Modules.Billing.Domain.ValueObjects;
using OmniCare.Modules.Billing.Features.GetInvoiceOwner;
using OmniCare.Modules.Billing.Infrastructure.Persistence;
using OmniCare.SharedKernel.Domain.ValueObjects;
using OmniCare.UnitTests.TestSupport;
using Xunit;

namespace OmniCare.UnitTests.Billing;

/// <summary>
/// GetInvoiceOwnerQuery est la vérification d'appartenance utilisée par le portail
/// patient (ticket #39) avant de servir le PDF d'une facture — un patient ne doit
/// jamais pouvoir consulter la facture d'un autre.
/// </summary>
public class GetInvoiceOwnerHandlerTests
{
    private static async Task<(BillingDbContext Context, Guid InvoiceId, Guid PatientId)> SeedAsync(SqliteConnection connection)
    {
        var options = new DbContextOptionsBuilder<BillingDbContext>().UseSqlite(connection).Options;
        var context = new BillingDbContext(options, new NoOpPublisher());
        await context.Database.MigrateAsync();

        var patientId = Guid.NewGuid();
        var invoice = Invoice.CreateNew(
            patientId, Guid.NewGuid(), InamiCode.Create("560011"), Amount.Create(25.50m),
            insuredAtIssue: true, preferentialRateAtIssue: false, patientShare: Amount.Create(25.50m),
            thirdPartyPayer: false);

        context.Invoices.Add(invoice);
        await context.SaveChangesAsync();

        return (context, invoice.Id, patientId);
    }

    [Fact]
    public async Task Returns_the_owning_patient_id()
    {
        using var connection = new SqliteConnection("DataSource=:memory:");
        await connection.OpenAsync();
        var (context, invoiceId, patientId) = await SeedAsync(connection);

        var handler = new GetInvoiceOwnerHandler(context);
        var result = await handler.Handle(new GetInvoiceOwnerQuery(invoiceId));

        Assert.True(result.IsSuccess);
        Assert.Equal(patientId, result.Value);
    }

    [Fact]
    public async Task Unknown_invoice_returns_failure()
    {
        using var connection = new SqliteConnection("DataSource=:memory:");
        await connection.OpenAsync();
        var (context, _, _) = await SeedAsync(connection);

        var handler = new GetInvoiceOwnerHandler(context);
        var result = await handler.Handle(new GetInvoiceOwnerQuery(Guid.NewGuid()));

        Assert.False(result.IsSuccess);
    }
}
