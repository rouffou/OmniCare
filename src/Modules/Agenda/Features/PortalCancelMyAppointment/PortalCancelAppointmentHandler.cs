using Mediarq.Core.Common.Requests.Command;
using Mediarq.Core.Common.Results;
using Microsoft.EntityFrameworkCore;
using OmniCare.Modules.Agenda.Domain.Entities;
using OmniCare.Modules.Agenda.Infrastructure.Persistence;
using OmniCare.SharedKernel.Application;

namespace OmniCare.Modules.Agenda.Features.PortalCancelMyAppointment;

public class PortalCancelAppointmentHandler : ICommandHandler<PortalCancelAppointmentCommand, Result<Guid>>
{
    private readonly IAgendaDbContext _context;

    public PortalCancelAppointmentHandler(IAgendaDbContext context)
    {
        _context = context;
    }

    public async Task<Result<Guid>> Handle(
        PortalCancelAppointmentCommand request, CancellationToken cancellationToken = default)
    {
        var appointment = await _context.Appointments
            .FirstOrDefaultAsync(a => a.Id == request.AppointmentId, cancellationToken);

        // Même message pour "inexistant" et "appartient à un autre patient" : ne jamais
        // révéler à un patient qu'un rendez-vous identifié par cet id existe ailleurs.
        if (appointment is null || appointment.PatientId != request.PatientId)
            return BusinessFailures.NotFound<Guid>($"Rendez-vous {request.AppointmentId} introuvable.");

        if (appointment.Status is not (AppointmentStatus.Planned or AppointmentStatus.Confirmed))
            return BusinessFailures.Rule<Guid>(
                $"Impossible d'annuler un rendez-vous au statut {appointment.Status}.");

        appointment.Cancel(request.Reason);

        return Result.Success(appointment.Id);
    }
}
