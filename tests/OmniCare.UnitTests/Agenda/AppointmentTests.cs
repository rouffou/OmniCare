using OmniCare.Modules.Agenda.Domain.Entities;
using OmniCare.Modules.Agenda.Domain.Events;
using OmniCare.Modules.Agenda.Domain.ValueObjects;
using OmniCare.SharedKernel.Domain;
using Xunit;

namespace OmniCare.UnitTests.Agenda;

public class AppointmentTests
{
    private static TimeSlot Tomorrow(int hour = 9) =>
        TimeSlot.FromDuration(
            new DateTimeOffset(DateTime.UtcNow.Date.AddDays(1).AddHours(hour), TimeSpan.Zero),
            TimeSpan.FromMinutes(30));

    private static Appointment NewAppointment() =>
        Appointment.Schedule(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Tomorrow());

    [Fact]
    public void Schedule_starts_planned_and_raises_event()
    {
        var appointment = NewAppointment();
        Assert.Equal(AppointmentStatus.Planned, appointment.Status);
        Assert.Contains(appointment.DomainEvents, e => e is AppointmentScheduledEvent);
    }

    [Fact]
    public void Schedule_requires_practitioner_patient_and_type()
    {
        Assert.Throws<DomainException>(
            () => Appointment.Schedule(Guid.Empty, Guid.NewGuid(), Guid.NewGuid(), Tomorrow()));
        Assert.Throws<DomainException>(
            () => Appointment.Schedule(Guid.NewGuid(), Guid.Empty, Guid.NewGuid(), Tomorrow()));
        Assert.Throws<DomainException>(
            () => Appointment.Schedule(Guid.NewGuid(), Guid.NewGuid(), Guid.Empty, Tomorrow()));
    }

    [Fact]
    public void Full_happy_path_planned_confirmed_completed()
    {
        var appointment = NewAppointment();
        appointment.Confirm();
        Assert.Equal(AppointmentStatus.Confirmed, appointment.Status);
        appointment.MarkAsCompleted();
        Assert.Equal(AppointmentStatus.Completed, appointment.Status);
        Assert.Contains(appointment.DomainEvents, e => e is AppointmentCompletedEvent);
    }

    [Fact]
    public void Cancel_records_reason_and_raises_event()
    {
        var appointment = NewAppointment();
        appointment.Cancel("Patient souffrant");
        Assert.Equal(AppointmentStatus.Cancelled, appointment.Status);
        Assert.Equal("Patient souffrant", appointment.CancellationReason);
        Assert.Contains(appointment.DomainEvents, e => e is AppointmentCancelledEvent);
    }

    [Fact]
    public void Completed_appointment_cannot_be_cancelled_or_reconfirmed()
    {
        var appointment = NewAppointment();
        appointment.MarkAsCompleted();
        Assert.Throws<DomainException>(() => appointment.Cancel(null));
        Assert.Throws<DomainException>(() => appointment.Confirm());
        Assert.Throws<DomainException>(() => appointment.MarkAsNoShow());
    }

    [Fact]
    public void Reschedule_resets_confirmation()
    {
        var appointment = NewAppointment();
        appointment.Confirm();
        appointment.Reschedule(Tomorrow(14));
        Assert.Equal(AppointmentStatus.Planned, appointment.Status);
    }

    [Fact]
    public void Cancelled_appointment_cannot_be_rescheduled()
    {
        var appointment = NewAppointment();
        appointment.Cancel(null);
        Assert.Throws<DomainException>(() => appointment.Reschedule(Tomorrow(14)));
    }

    [Fact]
    public void MarkReminderSent_records_timestamp_and_raises_event()
    {
        var appointment = NewAppointment();
        appointment.MarkReminderSent();
        Assert.NotNull(appointment.ReminderSentOn);
        Assert.Contains(appointment.DomainEvents, e => e is AppointmentReminderSentEvent);
    }

    [Fact]
    public void MarkReminderSent_twice_throws()
    {
        var appointment = NewAppointment();
        appointment.MarkReminderSent();
        Assert.Throws<DomainException>(() => appointment.MarkReminderSent());
    }

    [Fact]
    public void Reschedule_clears_previous_reminder()
    {
        var appointment = NewAppointment();
        appointment.MarkReminderSent();
        appointment.Reschedule(Tomorrow(14));
        Assert.Null(appointment.ReminderSentOn);
    }
}
