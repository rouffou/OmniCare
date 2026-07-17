using OmniCare.SharedKernel.Domain;

namespace OmniCare.Modules.Agenda.Domain.Events;

public sealed record AppointmentScheduledEvent(
    Guid AppointmentId,
    Guid PatientId,
    Guid PractitionerId,
    DateTimeOffset Start) : IDomainEvent
{
    public DateTimeOffset OccurredOn { get; } = DateTimeOffset.UtcNow;
}

public sealed record AppointmentCancelledEvent(
    Guid AppointmentId,
    Guid PatientId,
    Guid PractitionerId,
    string? Reason) : IDomainEvent
{
    public DateTimeOffset OccurredOn { get; } = DateTimeOffset.UtcNow;
}

public sealed record AppointmentCompletedEvent(
    Guid AppointmentId,
    Guid PatientId,
    Guid PractitionerId) : IDomainEvent
{
    public DateTimeOffset OccurredOn { get; } = DateTimeOffset.UtcNow;
}
