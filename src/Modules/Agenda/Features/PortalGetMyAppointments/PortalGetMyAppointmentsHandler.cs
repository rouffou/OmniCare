using Mediarq.Core.Common.Requests.Query;
using Mediarq.Core.Common.Results;
using Microsoft.EntityFrameworkCore;
using OmniCare.Modules.Agenda.Infrastructure.Persistence;
using OmniCare.SharedKernel.Application;

namespace OmniCare.Modules.Agenda.Features.PortalGetMyAppointments;

public class PortalGetMyAppointmentsHandler
    : IQueryHandler<PortalGetMyAppointmentsQuery, Result<IReadOnlyList<PortalAppointmentDto>>>
{
    private readonly IAgendaDbContext _context;
    private readonly IPractitionerDirectory _practitionerDirectory;

    public PortalGetMyAppointmentsHandler(IAgendaDbContext context, IPractitionerDirectory practitionerDirectory)
    {
        _context = context;
        _practitionerDirectory = practitionerDirectory;
    }

    public async Task<Result<IReadOnlyList<PortalAppointmentDto>>> Handle(
        PortalGetMyAppointmentsQuery request, CancellationToken cancellationToken = default)
    {
        var appointments = await _context.Appointments
            .AsNoTracking()
            .Where(a => a.PatientId == request.PatientId)
            .OrderByDescending(a => a.Slot.Start)
            .Join(
                _context.AppointmentTypes.AsNoTracking(),
                a => a.AppointmentTypeId,
                t => t.Id,
                (a, t) => new
                {
                    a.Id,
                    a.PractitionerId,
                    TypeName = t.Name,
                    a.Slot.Start,
                    a.Slot.End,
                    a.Status,
                })
            .ToListAsync(cancellationToken);

        var practitionerNames = new Dictionary<Guid, string>();
        var items = new List<PortalAppointmentDto>(appointments.Count);
        foreach (var appointment in appointments)
        {
            if (!practitionerNames.TryGetValue(appointment.PractitionerId, out var practitionerName))
            {
                var identity = await _practitionerDirectory.GetIdentityAsync(appointment.PractitionerId, cancellationToken);
                practitionerName = identity?.FullName ?? "Praticien inconnu";
                practitionerNames[appointment.PractitionerId] = practitionerName;
            }

            items.Add(new PortalAppointmentDto(
                appointment.Id,
                appointment.PractitionerId,
                practitionerName,
                appointment.TypeName,
                appointment.Start,
                appointment.End,
                appointment.Status.ToString()));
        }

        return Result.Success<IReadOnlyList<PortalAppointmentDto>>(items);
    }
}
