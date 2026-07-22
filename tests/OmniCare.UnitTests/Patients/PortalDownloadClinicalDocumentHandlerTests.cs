using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using OmniCare.Modules.Patients.Domain.Entities;
using OmniCare.Modules.Patients.Features.PortalDownloadMyClinicalDocument;
using OmniCare.Modules.Patients.Infrastructure.Persistence;
using OmniCare.Modules.Patients.Infrastructure.Storage;
using OmniCare.SharedKernel.Domain.ValueObjects;
using OmniCare.UnitTests.TestSupport;
using Xunit;

namespace OmniCare.UnitTests.Patients;

/// <summary>
/// Vérifie la vérification d'appartenance ajoutée pour le portail patient (ticket #39) :
/// contrairement à DownloadClinicalDocumentHandler (accès praticien, aucun scoping), un
/// patient ne doit jamais pouvoir télécharger le document d'un autre patient.
/// </summary>
public class PortalDownloadClinicalDocumentHandlerTests
{
    private sealed class FakeDocumentStorage : IDocumentStorage
    {
        private readonly Dictionary<string, byte[]> _files = [];

        public Task<string> SaveAsync(byte[] content, CancellationToken cancellationToken = default)
        {
            var key = Guid.NewGuid().ToString();
            _files[key] = content;
            return Task.FromResult(key);
        }

        public Task<byte[]> ReadAsync(string storageKey, CancellationToken cancellationToken = default) =>
            Task.FromResult(_files[storageKey]);

        public Task DeleteAsync(string storageKey, CancellationToken cancellationToken = default) =>
            Task.CompletedTask;
    }

    private static async Task<(PatientsDbContext Context, Guid PatientAId, Guid DocumentAId, Guid PatientBId)> SeedAsync(
        SqliteConnection connection, IDocumentStorage storage)
    {
        var options = new DbContextOptionsBuilder<PatientsDbContext>().UseSqlite(connection).Options;
        var context = new PatientsDbContext(options, new NoOpPublisher());
        await context.Database.MigrateAsync();

        var patientAId = Guid.NewGuid();
        var patientBId = Guid.NewGuid();

        var recordA = ClinicalRecord.Open(patientAId, HealthProfession.Physiotherapy);
        var storageKey = await storage.SaveAsync([1, 2, 3]);
        var documentA = recordA.AddDocument(
            ClinicalDocumentType.Prescription, "prescription.pdf", "application/pdf", 3, storageKey, Guid.NewGuid());

        var recordB = ClinicalRecord.Open(patientBId, HealthProfession.Physiotherapy);

        context.ClinicalRecords.AddRange(recordA, recordB);
        await context.SaveChangesAsync();

        return (context, patientAId, documentA.Id, patientBId);
    }

    [Fact]
    public async Task Owner_can_download_their_own_document()
    {
        using var connection = new SqliteConnection("DataSource=:memory:");
        await connection.OpenAsync();
        var storage = new FakeDocumentStorage();
        var (context, patientAId, documentAId, _) = await SeedAsync(connection, storage);

        var handler = new PortalDownloadClinicalDocumentHandler(context, storage);
        var result = await handler.Handle(new PortalDownloadClinicalDocumentQuery(documentAId, patientAId));

        Assert.True(result.IsSuccess);
        Assert.Equal([1, 2, 3], result.Value.Content);
    }

    [Fact]
    public async Task Another_patient_cannot_download_someone_elses_document()
    {
        using var connection = new SqliteConnection("DataSource=:memory:");
        await connection.OpenAsync();
        var storage = new FakeDocumentStorage();
        var (context, _, documentAId, patientBId) = await SeedAsync(connection, storage);

        var handler = new PortalDownloadClinicalDocumentHandler(context, storage);
        var result = await handler.Handle(new PortalDownloadClinicalDocumentQuery(documentAId, patientBId));

        Assert.False(result.IsSuccess);
    }
}
