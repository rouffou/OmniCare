using OmniCare.SharedKernel.Domain;

namespace OmniCare.Modules.Agenda.Domain.Entities;

/// <summary>
/// Demande d'un patient en liste d'attente pour un créneau qui se libérerait
/// chez un praticien donné (cahier des charges §4.2, gestion des désistements).
/// Processus manuel côté secrétariat : aucune notification automatique tant
/// que le canal SMS/email n'est pas branché (cf. ticket rappels automatiques).
/// </summary>
public sealed class WaitlistEntry : Entity
{
    public Guid PatientId { get; private set; }
    public Guid PractitionerId { get; private set; }
    public Guid AppointmentTypeId { get; private set; }
    public DateTimeOffset RequestedFrom { get; private set; }
    public DateTimeOffset RequestedTo { get; private set; }
    public string? Notes { get; private set; }
    public WaitlistStatus Status { get; private set; }
    public DateTimeOffset JoinedOn { get; private set; }

#pragma warning disable CS8618 // Constructeur de matérialisation EF Core
    private WaitlistEntry()
    {
    }
#pragma warning restore CS8618

    public static WaitlistEntry Join(
        Guid patientId,
        Guid practitionerId,
        Guid appointmentTypeId,
        DateTimeOffset requestedFrom,
        DateTimeOffset requestedTo,
        string? notes)
    {
        if (patientId == Guid.Empty)
            throw new DomainException("Une inscription en liste d'attente doit être rattachée à un patient.");
        if (practitionerId == Guid.Empty)
            throw new DomainException("Une inscription en liste d'attente doit être rattachée à un praticien.");
        if (appointmentTypeId == Guid.Empty)
            throw new DomainException("Une inscription en liste d'attente doit avoir un type d'acte.");
        if (requestedTo <= requestedFrom)
            throw new DomainException("La fin de la période souhaitée doit être postérieure à son début.");

        return new WaitlistEntry
        {
            PatientId = patientId,
            PractitionerId = practitionerId,
            AppointmentTypeId = appointmentTypeId,
            RequestedFrom = requestedFrom,
            RequestedTo = requestedTo,
            Notes = string.IsNullOrWhiteSpace(notes) ? null : notes.Trim(),
            Status = WaitlistStatus.Waiting,
            JoinedOn = DateTimeOffset.UtcNow,
        };
    }

    public void Withdraw()
    {
        if (Status != WaitlistStatus.Waiting)
            throw new DomainException($"Impossible de retirer une inscription au statut {Status}.");
        Status = WaitlistStatus.Withdrawn;
    }

    /// <summary>
    /// À appeler une fois qu'un créneau libéré a été manuellement réservé pour ce
    /// patient (via ScheduleAppointment) — sort l'entrée de la liste d'attente active.
    /// </summary>
    public void MarkFulfilled()
    {
        if (Status != WaitlistStatus.Waiting)
            throw new DomainException($"Impossible de clôturer une inscription au statut {Status}.");
        Status = WaitlistStatus.Fulfilled;
    }
}

public enum WaitlistStatus
{
    Waiting = 0,
    Fulfilled = 1,
    Withdrawn = 2,
}
