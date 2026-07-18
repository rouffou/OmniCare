using Mediarq.Core.Common.Requests.Command;
using Mediarq.Core.Common.Results;
using Microsoft.EntityFrameworkCore;
using OmniCare.Modules.Agenda.Domain.Entities;
using OmniCare.Modules.Agenda.Infrastructure.Persistence;
using OmniCare.SharedKernel.Application;

namespace OmniCare.Modules.Agenda.Features.JoinWaitlist;

public class JoinWaitlistHandler : ICommandHandler<JoinWaitlistCommand, Result<Guid>>
{
    private readonly IAgendaDbContext _context;
    private readonly IPatientDirectory _patients;

    public JoinWaitlistHandler(IAgendaDbContext context, IPatientDirectory patients)
    {
        _context = context;
        _patients = patients;
    }

    public async Task<Result<Guid>> Handle(JoinWaitlistCommand request, CancellationToken cancellationToken = default)
    {
        if (!await _patients.ExistsAsync(request.PatientId, cancellationToken))
            return BusinessFailures.NotFound<Guid>($"Patient {request.PatientId} introuvable.");

        var type = await _context.AppointmentTypes
            .AsNoTracking()
            .FirstOrDefaultAsync(t => t.Id == request.AppointmentTypeId, cancellationToken);
        if (type is null)
            return BusinessFailures.NotFound<Guid>($"Type de rendez-vous {request.AppointmentTypeId} introuvable.");

        var entry = WaitlistEntry.Join(
            request.PatientId, request.PractitionerId, request.AppointmentTypeId,
            request.RequestedFromUtc, request.RequestedToUtc, request.Notes);

        _context.WaitlistEntries.Add(entry);

        return Result.Success(entry.Id);
    }
}
