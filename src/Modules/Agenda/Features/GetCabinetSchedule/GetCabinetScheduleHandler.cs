using Mediarq.Core.Common.Requests.Query;
using Mediarq.Core.Common.Results;
using Microsoft.EntityFrameworkCore;
using OmniCare.Modules.Agenda.Infrastructure.Persistence;

namespace OmniCare.Modules.Agenda.Features.GetCabinetSchedule;

public class GetCabinetScheduleHandler
    : IQueryHandler<GetCabinetScheduleQuery, Result<IReadOnlyList<PractitionerScheduleDto>>>
{
    private readonly IAgendaDbContext _context;

    public GetCabinetScheduleHandler(IAgendaDbContext context)
    {
        _context = context;
    }

    public async Task<Result<IReadOnlyList<PractitionerScheduleDto>>> Handle(
        GetCabinetScheduleQuery request,
        CancellationToken cancellationToken = default)
    {
        var appointments = await _context.Appointments
            .AsNoTracking()
            .Where(a => a.Slot.Start < request.ToUtc && a.Slot.End > request.FromUtc)
            .OrderBy(a => a.Slot.Start)
            .Join(
                _context.AppointmentTypes.AsNoTracking(),
                a => a.AppointmentTypeId,
                t => t.Id,
                (a, t) => new
                {
                    a.PractitionerId,
                    Dto = new CabinetAppointmentDto(
                        a.Id, a.PatientId, a.AppointmentTypeId, t.Name,
                        a.Slot.Start, a.Slot.End, a.Status.ToString()),
                })
            .ToListAsync(cancellationToken);

        IReadOnlyList<PractitionerScheduleDto> schedule = appointments
            .GroupBy(x => x.PractitionerId)
            .Select(g => new PractitionerScheduleDto(g.Key, g.Select(x => x.Dto).ToList()))
            .OrderBy(p => p.PractitionerId)
            .ToList();

        return Result.Success(schedule);
    }
}
