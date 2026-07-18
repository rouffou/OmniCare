using OmniCare.Modules.Agenda.Domain.Events;
using OmniCare.Modules.Agenda.Domain.ValueObjects;
using OmniCare.SharedKernel.Domain;

namespace OmniCare.Modules.Agenda.Domain.Entities;

/// <summary>
/// Rendez-vous entre un patient et un praticien. Machine à états :
/// Planned → Confirmed → Completed, avec sorties Cancelled et NoShow
/// (cahier des charges §4.2). Les transitions invalides lèvent une DomainException.
/// </summary>
public sealed class Appointment : AggregateRoot
{
    public Guid PractitionerId { get; private set; }
    public Guid PatientId { get; private set; }
    public Guid AppointmentTypeId { get; private set; }

    /// <summary>Identifiant commun aux rendez-vous d'une même série récurrente (null si occurrence isolée).</summary>
    public Guid? SeriesId { get; private set; }

    public TimeSlot Slot { get; private set; }
    public AppointmentStatus Status { get; private set; }
    public string? Notes { get; private set; }
    public string? CancellationReason { get; private set; }
    public DateTimeOffset CreatedOn { get; private set; }

#pragma warning disable CS8618 // Constructeur de matérialisation EF Core
    private Appointment()
    {
    }
#pragma warning restore CS8618

    public static Appointment Schedule(
        Guid practitionerId,
        Guid patientId,
        Guid appointmentTypeId,
        TimeSlot slot,
        string? notes = null,
        Guid? seriesId = null)
    {
        if (practitionerId == Guid.Empty)
            throw new DomainException("Un rendez-vous doit être rattaché à un praticien.");
        if (patientId == Guid.Empty)
            throw new DomainException("Un rendez-vous doit être rattaché à un patient.");
        if (appointmentTypeId == Guid.Empty)
            throw new DomainException("Un rendez-vous doit avoir un type d'acte.");

        var appointment = new Appointment
        {
            PractitionerId = practitionerId,
            PatientId = patientId,
            AppointmentTypeId = appointmentTypeId,
            SeriesId = seriesId,
            Slot = slot,
            Status = AppointmentStatus.Planned,
            Notes = string.IsNullOrWhiteSpace(notes) ? null : notes.Trim(),
            CreatedOn = DateTimeOffset.UtcNow,
        };
        appointment.Raise(new AppointmentScheduledEvent(
            appointment.Id, patientId, practitionerId, slot.Start));
        return appointment;
    }

    public void Confirm()
    {
        EnsureStatus(AppointmentStatus.Planned);
        Status = AppointmentStatus.Confirmed;
    }

    public void Reschedule(TimeSlot newSlot)
    {
        if (Status is not (AppointmentStatus.Planned or AppointmentStatus.Confirmed))
            throw new DomainException($"Impossible de déplacer un rendez-vous au statut {Status}.");
        Slot = newSlot;
        // Un déplacement invalide la confirmation : le patient doit reconfirmer.
        Status = AppointmentStatus.Planned;
    }

    public void MarkAsCompleted()
    {
        if (Status is not (AppointmentStatus.Planned or AppointmentStatus.Confirmed))
            throw new DomainException($"Impossible de clôturer un rendez-vous au statut {Status}.");
        Status = AppointmentStatus.Completed;
        Raise(new AppointmentCompletedEvent(Id, PatientId, PractitionerId));
    }

    public void Cancel(string? reason)
    {
        if (Status is not (AppointmentStatus.Planned or AppointmentStatus.Confirmed))
            throw new DomainException($"Impossible d'annuler un rendez-vous au statut {Status}.");
        Status = AppointmentStatus.Cancelled;
        CancellationReason = string.IsNullOrWhiteSpace(reason) ? null : reason.Trim();
        Raise(new AppointmentCancelledEvent(Id, PatientId, PractitionerId, CancellationReason));
    }

    public void MarkAsNoShow()
    {
        if (Status is not (AppointmentStatus.Planned or AppointmentStatus.Confirmed))
            throw new DomainException($"Impossible de marquer no-show un rendez-vous au statut {Status}.");
        Status = AppointmentStatus.NoShow;
    }

    private void EnsureStatus(AppointmentStatus expected)
    {
        if (Status != expected)
            throw new DomainException(
                $"Transition invalide : statut attendu {expected}, statut actuel {Status}.");
    }
}

public enum AppointmentStatus
{
    Planned = 0,
    Confirmed = 1,
    Completed = 2,
    Cancelled = 3,
    NoShow = 4,
}
