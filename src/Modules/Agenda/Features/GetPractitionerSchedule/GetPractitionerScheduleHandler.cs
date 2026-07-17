using Mediarq.Core.Common.Requests.Query;
using Mediarq.Core.Common.Results;
using Microsoft.EntityFrameworkCore;
using OmniCare.Modules.Agenda.Infrastructure.Persistence;

namespace OmniCare.Modules.Agenda.Features.GetPractitionerSchedule;

public class GetPractitionerScheduleHandler
    : IQueryHandler<GetPractitionerScheduleQuery, Result<IReadOnlyList<AppointmentDto>>>
{
    private readonly IAgendaDbContext _context;

    public GetPractitionerScheduleHandler(IAgendaDbContext context)
    {
        _context = context;
    }

    public async Task<Result<IReadOnlyList<AppointmentDto>>> Handle(
        GetPractitionerScheduleQuery request,
        CancellationToken cancellationToken = default)
    {
        var items = await _context.Appointments
            .AsNoTracking()
            .Where(a => a.PractitionerId == request.PractitionerId &&
                        a.Slot.Start < request.ToUtc &&
                        a.Slot.End > request.FromUtc)
            .OrderBy(a => a.Slot.Start)
            .Join(
                _context.AppointmentTypes.AsNoTracking(),
                a => a.AppointmentTypeId,
                t => t.Id,
                (a, t) => new AppointmentDto(
                    a.Id,
                    a.PatientId,
                    a.AppointmentTypeId,
                    t.Name,
                    a.Slot.Start,
                    a.Slot.End,
                    a.Status.ToString(),
                    a.Notes))
            .ToListAsync(cancellationToken);

        return Result.Success<IReadOnlyList<AppointmentDto>>(items);
    }
}
