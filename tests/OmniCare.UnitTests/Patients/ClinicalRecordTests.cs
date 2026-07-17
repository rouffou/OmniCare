using OmniCare.Modules.Patients.Domain.Entities;
using OmniCare.Modules.Patients.Domain.Events;
using OmniCare.SharedKernel.Domain;
using OmniCare.SharedKernel.Domain.ValueObjects;
using Xunit;

namespace OmniCare.UnitTests.Patients;

public class ClinicalRecordTests
{
    private static ClinicalRecord NewRecord() =>
        ClinicalRecord.Open(Guid.NewGuid(), HealthProfession.Physiotherapy);

    [Fact]
    public void Open_requires_a_patient()
    {
        Assert.Throws<DomainException>(
            () => ClinicalRecord.Open(Guid.Empty, HealthProfession.Physiotherapy));
    }

    [Fact]
    public void AddEntry_records_author_and_raises_event()
    {
        var record = NewRecord();
        var practitionerId = Guid.NewGuid();

        var entry = record.AddEntry(
            ClinicalEntryType.ClinicalAssessment, "Bilan initial : mobilité réduite.", practitionerId);

        Assert.Single(record.Entries);
        Assert.Equal(practitionerId, entry.AuthorPractitionerId);
        Assert.Contains(record.DomainEvents, e =>
            e is ClinicalEntryAddedEvent added && added.EntryId == entry.Id);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void AddEntry_rejects_empty_content(string content)
    {
        var record = NewRecord();
        Assert.Throws<DomainException>(
            () => record.AddEntry(ClinicalEntryType.SessionReport, content, Guid.NewGuid()));
    }

    [Fact]
    public void AddEntry_rejects_missing_author()
    {
        var record = NewRecord();
        Assert.Throws<DomainException>(
            () => record.AddEntry(ClinicalEntryType.SessionReport, "Séance 1", Guid.Empty));
    }

    [Fact]
    public void RegisterPrescription_validates_invariants()
    {
        var record = NewRecord();
        Assert.Throws<DomainException>(
            () => record.RegisterPrescription("", new DateOnly(2026, 7, 1), 9));
        Assert.Throws<DomainException>(
            () => record.RegisterPrescription("Dr Janssens", new DateOnly(2026, 7, 1), 0));
    }

    [Fact]
    public void ConsumeSession_uses_oldest_open_prescription_first()
    {
        var record = NewRecord();
        var older = record.RegisterPrescription("Dr Janssens", new DateOnly(2026, 5, 1), 1);
        var newer = record.RegisterPrescription("Dr Janssens", new DateOnly(2026, 7, 1), 9);

        var consumedFrom = record.ConsumeSessionFromOldestOpenPrescription();
        Assert.Same(older, consumedFrom);
        Assert.Equal(0, older.RemainingSessions);

        consumedFrom = record.ConsumeSessionFromOldestOpenPrescription();
        Assert.Same(newer, consumedFrom);
        Assert.Equal(8, newer.RemainingSessions);
    }

    [Fact]
    public void ConsumeSession_returns_null_when_everything_is_exhausted()
    {
        var record = NewRecord();
        record.RegisterPrescription("Dr Janssens", new DateOnly(2026, 7, 1), 1);

        Assert.NotNull(record.ConsumeSessionFromOldestOpenPrescription());
        Assert.Null(record.ConsumeSessionFromOldestOpenPrescription());
    }
}
