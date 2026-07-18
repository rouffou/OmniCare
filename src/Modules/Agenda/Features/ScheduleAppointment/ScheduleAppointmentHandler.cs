using Mediarq.Core.Common.Requests.Command;
using Mediarq.Core.Common.Results;
using Microsoft.EntityFrameworkCore;
using OmniCare.Modules.Agenda.Domain.Entities;
using OmniCare.Modules.Agenda.Domain.ValueObjects;
using OmniCare.Modules.Agenda.Infrastructure.Persistence;
using OmniCare.SharedKernel.Application;

namespace OmniCare.Modules.Agenda.Features.ScheduleAppointment;

public class ScheduleAppointmentHandler : ICommandHandler<ScheduleAppointmentCommand, Result<Guid>>
{
    private readonly IAgendaDbContext _context;

    public ScheduleAppointmentHandler(IAgendaDbContext context)
    {
        _context = context;
    }

    public async Task<Result<Guid>> Handle(ScheduleAppointmentCommand request, CancellationToken cancellationToken = default)
    {
        var type = await _context.AppointmentTypes
            .AsNoTracking()
            .FirstOrDefaultAsync(t => t.Id == request.AppointmentTypeId, cancellationToken);
        if (type is null)
            return BusinessFailures.NotFound<Guid>($"Type de rendez-vous {request.AppointmentTypeId} introuvable.");
        if (!type.IsActive)
            return BusinessFailures.Rule<Guid>($"Le type de rendez-vous « {type.Name} » est désactivé.");

        var slot = request.EndUtc.HasValue
            ? TimeSlot.Create(request.StartUtc, request.EndUtc.Value)
            : TimeSlot.FromDuration(request.StartUtc, type.DefaultDuration);

        // Chevauchement dans l'agenda du praticien (statuts encore « vivants » uniquement).
        var hasOverlap = await _context.Appointments.AnyAsync(a =>
            a.PractitionerId == request.PractitionerId &&
            (a.Status == AppointmentStatus.Planned || a.Status == AppointmentStatus.Confirmed) &&
            a.Slot.Start < slot.End && slot.Start < a.Slot.End,
            cancellationToken);
        if (hasOverlap)
            return BusinessFailures.Conflict<Guid>(
                "Le praticien a déjà un rendez-vous sur ce créneau.");

        if (request.RoomId.HasValue)
        {
            var room = await _context.Rooms
                .AsNoTracking()
                .FirstOrDefaultAsync(r => r.Id == request.RoomId, cancellationToken);
            if (room is null)
                return BusinessFailures.NotFound<Guid>($"Salle {request.RoomId} introuvable.");
            if (!room.IsActive)
                return BusinessFailures.Rule<Guid>($"La salle « {room.Name} » est désactivée.");

            var roomIsBusy = await _context.Appointments.AnyAsync(a =>
                a.RoomId == request.RoomId &&
                (a.Status == AppointmentStatus.Planned || a.Status == AppointmentStatus.Confirmed) &&
                a.Slot.Start < slot.End && slot.Start < a.Slot.End,
                cancellationToken);
            if (roomIsBusy)
                return BusinessFailures.Conflict<Guid>(
                    $"La salle « {room.Name} » est déjà réservée sur ce créneau.");
        }

        var appointment = Appointment.Schedule(
            request.PractitionerId, request.PatientId, request.AppointmentTypeId, slot, request.Notes, roomId: request.RoomId);

        _context.Appointments.Add(appointment);

        return Result.Success(appointment.Id);
    }
}
