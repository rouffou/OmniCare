using OmniCare.SharedKernel.Domain;

namespace OmniCare.Modules.Billing.Domain.Events;

/// <summary>
/// Événement critique du cycle de facturation : déclenchera à terme l'envoi de
/// l'eAttest vers eHealth/MyCareNet. À fiabiliser via Mediarq.Outbox lors du
/// branchement de la télétransmission (aucune perte tolérée — cf. architecture §4).
/// </summary>
public sealed record InvoiceGeneratedEvent(
    Guid InvoiceId,
    Guid PatientId,
    Guid PractitionerId,
    string InamiCode,
    decimal Amount) : IDomainEvent
{
    public DateTimeOffset OccurredOn { get; } = DateTimeOffset.UtcNow;
}

public sealed record InvoicePaidEvent(Guid InvoiceId, Guid PatientId) : IDomainEvent
{
    public DateTimeOffset OccurredOn { get; } = DateTimeOffset.UtcNow;
}

public sealed record InvoiceCancelledEvent(Guid InvoiceId, Guid PatientId, string? Reason) : IDomainEvent
{
    public DateTimeOffset OccurredOn { get; } = DateTimeOffset.UtcNow;
}
