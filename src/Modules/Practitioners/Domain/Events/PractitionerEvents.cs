using OmniCare.SharedKernel.Domain;

namespace OmniCare.Modules.Practitioners.Domain.Events;

public sealed record CabinetRegisteredEvent(Guid CabinetId) : IDomainEvent
{
    public DateTimeOffset OccurredOn { get; } = DateTimeOffset.UtcNow;
}

public sealed record PractitionerRegisteredEvent(Guid PractitionerId, Guid CabinetId) : IDomainEvent
{
    public DateTimeOffset OccurredOn { get; } = DateTimeOffset.UtcNow;
}
