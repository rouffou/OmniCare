using OmniCare.SharedKernel.Domain;

namespace OmniCare.Modules.Agenda.Domain.ValueObjects;

/// <summary>Créneau horaire d'un rendez-vous (UTC), auto-validant.</summary>
public sealed record TimeSlot
{
    public DateTimeOffset Start { get; private set; }
    public DateTimeOffset End { get; private set; }

    private TimeSlot(DateTimeOffset start, DateTimeOffset end)
    {
        Start = start;
        End = end;
    }

    public static TimeSlot Create(DateTimeOffset start, DateTimeOffset end)
    {
        if (end <= start)
            throw new DomainException("La fin d'un créneau doit être postérieure à son début.");
        if (end - start > TimeSpan.FromHours(8))
            throw new DomainException("Un créneau de rendez-vous ne peut pas excéder 8 heures.");
        return new TimeSlot(start, end);
    }

    public static TimeSlot FromDuration(DateTimeOffset start, TimeSpan duration) =>
        Create(start, start + duration);

    public TimeSpan Duration => End - Start;

    public bool Overlaps(TimeSlot other) => Start < other.End && other.Start < End;
}
