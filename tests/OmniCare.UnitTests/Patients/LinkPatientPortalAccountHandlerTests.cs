using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using OmniCare.Modules.Patients.Domain.Entities;
using OmniCare.Modules.Patients.Domain.ValueObjects;
using OmniCare.Modules.Patients.Features.LinkPatientPortalAccount;
using OmniCare.Modules.Patients.Infrastructure.Persistence;
using OmniCare.UnitTests.TestSupport;
using Xunit;

namespace OmniCare.UnitTests.Patients;

public class LinkPatientPortalAccountHandlerTests
{
    // Chaque patient est créé et sauvegardé via son propre DbContext (comme en production,
    // une requête HTTP = un DbContext) : Patient.Register() attribue InsurabilityStatus.Unknown
    // et ContactDetails.Empty, des singletons statiques partagés par toutes les instances —
    // EF Core ne supporte pas qu'un même type owned soit suivi par deux agrégats dans le
    // même DbContext, même via des SaveChanges séparés.
    private static PatientsDbContext NewContext(SqliteConnection connection) =>
        new(new DbContextOptionsBuilder<PatientsDbContext>().UseSqlite(connection).Options, new NoOpPublisher());

    private static async Task<Guid> RegisterPatientAsync(SqliteConnection connection, string firstName)
    {
        await using var context = NewContext(connection);
        var patient = Patient.Register(
            PersonName.Create(firstName, "Test"), nationalRegistryNumber: null, birthDate: null, ContactDetails.Empty);
        context.Patients.Add(patient);
        await context.SaveChangesAsync();
        return patient.Id;
    }

    private static async Task<SqliteConnection> SeedDatabaseAsync()
    {
        var connection = new SqliteConnection("DataSource=:memory:");
        await connection.OpenAsync();
        await using (var context = NewContext(connection))
            await context.Database.MigrateAsync();
        return connection;
    }

    [Fact]
    public async Task Links_the_portal_account_to_the_patient()
    {
        using var connection = await SeedDatabaseAsync();
        var patientAId = await RegisterPatientAsync(connection, "Alice");

        await using var context = NewContext(connection);
        var handler = new LinkPatientPortalAccountHandler(context);
        var result = await handler.Handle(new LinkPatientPortalAccountCommand(patientAId, "oidc|alice"));

        Assert.True(result.IsSuccess);
    }

    [Fact]
    public async Task Rejects_a_portal_account_already_linked_to_another_patient()
    {
        using var connection = await SeedDatabaseAsync();
        var patientAId = await RegisterPatientAsync(connection, "Alice");
        var patientBId = await RegisterPatientAsync(connection, "Bob");

        await using (var linkContext = NewContext(connection))
        {
            // Les handlers ne committent pas eux-mêmes (UnitOfWorkBehavior s'en charge dans
            // le pipeline Mediarq, contourné ici) : SaveChanges explicite pour persister le
            // premier lien avant de vérifier le rejet du second sur un contexte frais.
            await new LinkPatientPortalAccountHandler(linkContext)
                .Handle(new LinkPatientPortalAccountCommand(patientAId, "oidc|shared"));
            await linkContext.SaveChangesAsync();
        }

        await using var context = NewContext(connection);
        var handler = new LinkPatientPortalAccountHandler(context);
        var result = await handler.Handle(new LinkPatientPortalAccountCommand(patientBId, "oidc|shared"));

        Assert.False(result.IsSuccess);
    }
}
