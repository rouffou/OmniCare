using OmniCare.SharedKernel.Domain;

namespace OmniCare.Modules.Patients.Domain.Events;

public sealed record PatientRegisteredEvent(Guid PatientId) : IDomainEvent
{
    public DateTimeOffset OccurredOn { get; } = DateTimeOffset.UtcNow;
}

public sealed record PatientArchivedEvent(Guid PatientId) : IDomainEvent
{
    public DateTimeOffset OccurredOn { get; } = DateTimeOffset.UtcNow;
}

public sealed record ClinicalEntryAddedEvent(
    Guid ClinicalRecordId,
    Guid PatientId,
    Guid EntryId,
    Guid AuthorPractitionerId) : IDomainEvent
{
    public DateTimeOffset OccurredOn { get; } = DateTimeOffset.UtcNow;
}

public sealed record PrescriptionRegisteredEvent(
    Guid ClinicalRecordId,
    Guid PatientId,
    Guid PrescriptionId,
    int SessionsPrescribed) : IDomainEvent
{
    public DateTimeOffset OccurredOn { get; } = DateTimeOffset.UtcNow;
}

public sealed record ClinicalDocumentAddedEvent(
    Guid ClinicalRecordId,
    Guid PatientId,
    Guid DocumentId,
    Guid UploadedByPractitionerId) : IDomainEvent
{
    public DateTimeOffset OccurredOn { get; } = DateTimeOffset.UtcNow;
}
