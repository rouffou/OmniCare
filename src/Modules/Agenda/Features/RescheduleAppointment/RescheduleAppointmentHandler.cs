using Mediarq.Core.Common.Requests.Command;
using Mediarq.Core.Common.Results;
using Microsoft.EntityFrameworkCore;
using OmniCare.Modules.Agenda.Domain.Entities;
using OmniCare.Modules.Agenda.Domain.ValueObjects;
using OmniCare.Modules.Agenda.Infrastructure.Persistence;
using OmniCare.SharedKernel.Application;
using OmniCare.SharedKernel.Domain;

namespace OmniCare.Modules.Agenda.Features.RescheduleAppointment;

public class RescheduleAppointmentHandler : ICommandHandler<RescheduleAppointmentCommand, Result<Guid>>
{
    private readonly IAgendaDbContext _context;

    public RescheduleAppointmentHandler(IAgendaDbContext context)
    {
        _context = context;
    }

    public async Task<Result<Guid>> Handle(RescheduleAppointmentCommand request, CancellationToken cancellationToken = default)
    {
        var appointment = await _context.Appointments
            .FirstOrDefaultAsync(a => a.Id == request.AppointmentId, cancellationToken);
        if (appointment is null)
            return BusinessFailures.NotFound<Guid>($"Rendez-vous {request.AppointmentId} introuvable.");

        TimeSlot newSlot;
        if (request.EndUtc.HasValue)
        {
            newSlot = TimeSlot.Create(request.StartUtc, request.EndUtc.Value);
        }
        else
        {
            var type = await _context.AppointmentTypes
                .AsNoTracking()
                .FirstAsync(t => t.Id == appointment.AppointmentTypeId, cancellationToken);
            newSlot = TimeSlot.FromDuration(request.StartUtc, type.DefaultDuration);
        }

        // Chevauchement dans l'agenda du praticien, en excluant le rendez-vous déplacé.
        var hasOverlap = await _context.Appointments.AnyAsync(a =>
            a.Id != appointment.Id &&
            a.PractitionerId == appointment.PractitionerId &&
            (a.Status == AppointmentStatus.Planned || a.Status == AppointmentStatus.Confirmed) &&
            a.Slot.Start < newSlot.End && newSlot.Start < a.Slot.End,
            cancellationToken);
        if (hasOverlap)
            return BusinessFailures.Conflict<Guid>(
                "Le praticien a déjà un rendez-vous sur ce créneau.");

        if (appointment.RoomId.HasValue)
        {
            var roomIsBusy = await _context.Appointments.AnyAsync(a =>
                a.Id != appointment.Id &&
                a.RoomId == appointment.RoomId &&
                (a.Status == AppointmentStatus.Planned || a.Status == AppointmentStatus.Confirmed) &&
                a.Slot.Start < newSlot.End && newSlot.Start < a.Slot.End,
                cancellationToken);
            if (roomIsBusy)
                return BusinessFailures.Conflict<Guid>("La salle réservée est déjà occupée sur ce créneau.");
        }

        try
        {
            appointment.Reschedule(newSlot);
        }
        catch (DomainException ex)
        {
            // Rendez-vous déjà clôturé/annulé : déplacement impossible.
            return BusinessFailures.Rule<Guid>(ex.Message);
        }

        return Result.Success(appointment.Id);
    }
}
