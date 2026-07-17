using Mediarq.Core.Common.Requests.Command;
using Mediarq.Core.Common.Results;
using Microsoft.EntityFrameworkCore;
using OmniCare.Modules.Agenda.Infrastructure.Persistence;
using OmniCare.SharedKernel.Application;
using OmniCare.SharedKernel.Domain;

namespace OmniCare.Modules.Agenda.Features.ChangeAppointmentStatus;

public class ChangeAppointmentStatusHandler : ICommandHandler<ChangeAppointmentStatusCommand, Result<Guid>>
{
    private readonly IAgendaDbContext _context;

    public ChangeAppointmentStatusHandler(IAgendaDbContext context)
    {
        _context = context;
    }

    public async Task<Result<Guid>> Handle(ChangeAppointmentStatusCommand request, CancellationToken cancellationToken = default)
    {
        var appointment = await _context.Appointments
            .FirstOrDefaultAsync(a => a.Id == request.AppointmentId, cancellationToken);
        if (appointment is null)
            return BusinessFailures.NotFound<Guid>($"Rendez-vous {request.AppointmentId} introuvable.");

        try
        {
            switch (request.Transition.ToUpperInvariant())
            {
                case "CONFIRM":
                    appointment.Confirm();
                    break;
                case "COMPLETE":
                    appointment.MarkAsCompleted();
                    break;
                case "NOSHOW":
                    appointment.MarkAsNoShow();
                    break;
            }
        }
        catch (DomainException ex)
        {
            return BusinessFailures.Rule<Guid>(ex.Message);
        }
        return Result.Success(appointment.Id);
    }
}
