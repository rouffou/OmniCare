using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using OmniCare.Modules.Agenda.Domain.Entities;
using OmniCare.Modules.Agenda.Domain.ValueObjects;
using OmniCare.Modules.Agenda.Features.PortalCancelMyAppointment;
using OmniCare.Modules.Agenda.Infrastructure.Persistence;
using OmniCare.SharedKernel.Domain.ValueObjects;
using OmniCare.UnitTests.TestSupport;
using Xunit;

namespace OmniCare.UnitTests.Agenda;

/// <summary>
/// Vérifie la vérification d'appartenance ajoutée pour le portail patient (ticket #39) :
/// contrairement à CancelAppointmentHandler (accès praticien/secrétariat, aucun scoping),
/// un patient ne doit jamais pouvoir annuler le rendez-vous d'un autre patient.
/// </summary>
public class PortalCancelAppointmentHandlerTests
{
    private static TimeSlot Tomorrow() =>
        TimeSlot.FromDuration(
            new DateTimeOffset(DateTime.UtcNow.Date.AddDays(1).AddHours(9), TimeSpan.Zero),
            TimeSpan.FromMinutes(30));

    private static async Task<(AgendaDbContext Context, Guid AppointmentId, Guid PatientAId, Guid PatientBId)> SeedAsync(
        SqliteConnection connection)
    {
        var options = new DbContextOptionsBuilder<AgendaDbContext>().UseSqlite(connection).Options;
        var context = new AgendaDbContext(options, new NoOpPublisher());
        await context.Database.MigrateAsync();

        var appointmentType = AppointmentType.Define("Séance kiné", HealthProfession.Physiotherapy, TimeSpan.FromMinutes(30));
        context.AppointmentTypes.Add(appointmentType);
        await context.SaveChangesAsync();

        var patientAId = Guid.NewGuid();
        var patientBId = Guid.NewGuid();
        var appointment = Appointment.Schedule(Guid.NewGuid(), patientAId, appointmentType.Id, Tomorrow());

        context.Appointments.Add(appointment);
        await context.SaveChangesAsync();

        return (context, appointment.Id, patientAId, patientBId);
    }

    [Fact]
    public async Task Owner_can_cancel_their_own_appointment()
    {
        using var connection = new SqliteConnection("DataSource=:memory:");
        await connection.OpenAsync();
        var (context, appointmentId, patientAId, _) = await SeedAsync(connection);

        var handler = new PortalCancelAppointmentHandler(context);
        var result = await handler.Handle(new PortalCancelAppointmentCommand(appointmentId, patientAId, "Empêchement"));

        Assert.True(result.IsSuccess);
        Assert.Equal(AppointmentStatus.Cancelled, context.Appointments.Single().Status);
    }

    [Fact]
    public async Task Another_patient_cannot_cancel_someone_elses_appointment()
    {
        using var connection = new SqliteConnection("DataSource=:memory:");
        await connection.OpenAsync();
        var (context, appointmentId, _, patientBId) = await SeedAsync(connection);

        var handler = new PortalCancelAppointmentHandler(context);
        var result = await handler.Handle(new PortalCancelAppointmentCommand(appointmentId, patientBId, null));

        Assert.False(result.IsSuccess);
        Assert.Equal(AppointmentStatus.Planned, context.Appointments.Single().Status);
    }
}
