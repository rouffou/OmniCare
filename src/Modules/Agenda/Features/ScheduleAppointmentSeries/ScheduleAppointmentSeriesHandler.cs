using Mediarq.Core.Common.Requests.Command;
using Mediarq.Core.Common.Results;
using Microsoft.EntityFrameworkCore;
using OmniCare.Modules.Agenda.Domain.Entities;
using OmniCare.Modules.Agenda.Domain.ValueObjects;
using OmniCare.Modules.Agenda.Infrastructure.Persistence;
using OmniCare.SharedKernel.Application;

namespace OmniCare.Modules.Agenda.Features.ScheduleAppointmentSeries;

public class ScheduleAppointmentSeriesHandler
    : ICommandHandler<ScheduleAppointmentSeriesCommand, Result<AppointmentSeriesResult>>
{
    private readonly IAgendaDbContext _context;
    private readonly IPatientDirectory _patients;

    public ScheduleAppointmentSeriesHandler(IAgendaDbContext context, IPatientDirectory patients)
    {
        _context = context;
        _patients = patients;
    }

    public async Task<Result<AppointmentSeriesResult>> Handle(
        ScheduleAppointmentSeriesCommand request,
        CancellationToken cancellationToken = default)
    {
        if (!await _patients.ExistsAsync(request.PatientId, cancellationToken))
            return BusinessFailures.NotFound<AppointmentSeriesResult>($"Patient {request.PatientId} introuvable.");

        var type = await _context.AppointmentTypes
            .AsNoTracking()
            .FirstOrDefaultAsync(t => t.Id == request.AppointmentTypeId, cancellationToken);
        if (type is null)
            return BusinessFailures.NotFound<AppointmentSeriesResult>(
                $"Type de rendez-vous {request.AppointmentTypeId} introuvable.");
        if (!type.IsActive)
            return BusinessFailures.Rule<AppointmentSeriesResult>(
                $"Le type de rendez-vous « {type.Name} » est désactivé.");

        var duration = request.EndUtc.HasValue
            ? request.EndUtc.Value - request.FirstStartUtc
            : type.DefaultDuration;

        var seriesId = Guid.CreateVersion7();
        var appointments = new List<Appointment>();

        // Occurrences hebdomadaires (ou multiples de semaines) sur le même créneau horaire ;
        // le fuseau et l'heure de FirstStartUtc sont conservés pour chaque occurrence.
        for (var i = 0; i < request.OccurrenceCount; i++)
        {
            var start = request.FirstStartUtc.AddDays(7 * request.IntervalWeeks * i);
            var slot = TimeSlot.FromDuration(start, duration);

            var hasOverlap = await _context.Appointments.AnyAsync(a =>
                a.PractitionerId == request.PractitionerId &&
                (a.Status == AppointmentStatus.Planned || a.Status == AppointmentStatus.Confirmed) &&
                a.Slot.Start < slot.End && slot.Start < a.Slot.End,
                cancellationToken);
            if (hasOverlap)
                return BusinessFailures.Conflict<AppointmentSeriesResult>(
                    $"Le praticien a déjà un rendez-vous sur le créneau de l'occurrence {i + 1} ({start:yyyy-MM-dd HH:mm}). Aucune occurrence n'a été créée.");

            appointments.Add(Appointment.Schedule(
                request.PractitionerId, request.PatientId, request.AppointmentTypeId,
                slot, request.Notes, seriesId));
        }

        _context.Appointments.AddRange(appointments);

        return Result.Success(new AppointmentSeriesResult(seriesId, appointments.Select(a => a.Id).ToList()));
    }
}
